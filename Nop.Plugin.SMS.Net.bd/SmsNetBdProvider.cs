using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Net.bd
{
    /// <summary>
    /// Represents the Alpha SMS provider
    /// </summary>
    public class SmsNetBdProvider : BasePlugin, IMiscPlugin
    {
        private const string DefaultBaseUrl = "https://api.sms.net.bd/sendsms";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SmsNetBdProvider> _logger;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly SmsNetBdSettings _smsSettings;
        protected readonly ILocalizationService _localizationService;

        public SmsNetBdProvider(IHttpClientFactory httpClientFactory,
            ILogger<SmsNetBdProvider> logger,
            ISettingService settingService,
            IWebHelper webHelper,
            SmsNetBdSettings smsSettings,
            ILocalizationService localization)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _settingService = settingService;
            _webHelper = webHelper;
            _smsSettings = smsSettings;
            _localizationService = localization;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public async Task<bool> SendSmsAsync(string number, string message, string? senderId = null)
        {
            if (string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(message))
                return false;

            if (string.IsNullOrWhiteSpace(_smsSettings.API_Key))
                return false;

            var baseUrl = string.IsNullOrWhiteSpace(_smsSettings.API_Url) ? DefaultBaseUrl : _smsSettings.API_Url.Trim();
            baseUrl = baseUrl.TrimEnd('?');
            var payload = new Dictionary<string, string>
            {
                ["api_key"] = _smsSettings.API_Key,
                ["msg"] = message,
                ["to"] = number
            };

            var effectiveSender = !string.IsNullOrWhiteSpace(senderId) ? senderId : _smsSettings.sender_id;
            if (!string.IsNullOrWhiteSpace(effectiveSender))
                payload["sender_id"] = effectiveSender!;

            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(SmsNetBdProvider));
                var requestUri = QueryHelpers.AddQueryString(baseUrl, payload);
                using var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("sms.net.bd returned non-success status code {StatusCode}", response.StatusCode);
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(content))
                    return false;

                var parsed = JsonConvert.DeserializeObject<JObject>(content);
                var error = parsed?["error"]?.Value<string>();
                return string.Equals(error, "0", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS through sms.net.bd");
                return false;
            }
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SmsNetBd/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new SmsNetBdSettings
            {
                API_Url = DefaultBaseUrl,
                Enabled = true,
                EnabledConfirmOrder = true,
                EnabledOrderCanceled = true,
                EnabledOrderCompleted = false,
                EnabledOrderShipping = false,
                EnabledPaymented = false,
                EnabledRegistered = false,
                ConfirmOrderSMSForCustomerFormat = "Your Order is Confirmed. Order ID: %[ID]%. Total Amount: %[OrderTotal]%",
                ConfirmOrderSMSForOwnerFormat = "%[StoreName]% Order is Placed #%[ID]% and Total Amount: %[OrderTotal]%",
                OrderPaidSMSFormat = "[ %[StoreName]% ] We received your payment for order #{%[ID]%}",
                OrderCanceledSMSFormat = "[ %[StoreName]% ]  Your order #{%[ID]%}status has been updated to Canceled.",
                OrderRefundedSMSFormat = "[ %[StoreName]% ]  Order #{%[ID]%} has been refunded.",
                OrderShippingSMSFormat = "[ %[StoreName]% ]  Order #{%[ID]%} has been %[ShippingStatus]%"


            };
            await _settingService.SaveSettingAsync(settings).ConfigureAwait(false);
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.SMS.Net.bd.Fields.Api_Url"] = "API URL",
                ["Plugins.SMS.Net.bd.Fields.Api_Url.Hint"] = "The SMS provider’s API endpoint. Example: https://api.sms.net.bd/sendsms",

                ["Plugins.SMS.Net.bd.Fields.API_Key"] = "API Key",
                ["Plugins.SMS.Net.bd.Fields.API_Key.Hint"] = "Enter the API key provided by your service provider.",

                ["Plugins.SMS.Net.bd.Fields.Enabled"] = "Enable Plugin",
                ["Plugins.SMS.Net.bd.Fields.Enabled.Hint"] = "Check to enable the SMS.Net.bd plugin. If unchecked, the plugin will be disabled.",

                ["Plugins.SMS.Net.bd.Fields.sender_id"] = "Sender ID",
                ["Plugins.SMS.Net.bd.Fields.sender_id.Hint"] = "The sender ID (masking) registered with your SMS provider.",

                ["Plugins.SMS.Net.bd.Fields.CustomerEnabled"] = "Customer SMS Enabled",
                ["Plugins.SMS.Net.bd.Fields.CustomerEnabled.Hint"] = "If checked, customers will receive SMS notifications for order events.",

                ["Plugins.SMS.Net.bd.Fields.OwnerEnabled"] = "Owner SMS Enabled",
                ["Plugins.SMS.Net.bd.Fields.OwnerEnabled.Hint"] = "If checked, the store owner will receive SMS notifications for order events.",

                ["Plugins.SMS.Net.bd.Fields.OwnerNumber"] = "Owner Number",
                ["Plugins.SMS.Net.bd.Fields.OwnerNumber.Hint"] = "Enter the mobile number(s) where the store owner should receive SMS alerts.",

                ["Plugins.SMS.Net.bd.Fields.EnabledConfirmOrder"] = "Enable Confirm Order",
                ["Plugins.SMS.Net.bd.Fields.EnabledConfirmOrder.Hint"] = "If enabled, SMS messages will be sent when an order is confirmed.",

                ["Plugins.SMS.Net.bd.Fields.SendToCustomerConfirmOrderSMSEnabled"] = "Customer Confirm Order SMS Enabled",
                ["Plugins.SMS.Net.bd.Fields.SendToCustomerConfirmOrderSMSEnabled.Hint"] = "If enabled, SMS messages will be sent when an order is confirmed.",

                ["Plugins.SMS.Net.bd.Fields.ConfirmOrderSMSForCustomerFormat"] = "Customer Confirm Order SMS Format",
                ["Plugins.SMS.Net.bd.Fields.ConfirmOrderSMSForCustomerFormat.Hint"] = "Template for SMS sent to store customer on order confirmation.",

                ["Plugins.SMS.Net.bd.Fields.EnableOrderPaid"] = "Enable Order Paid",
                ["Plugins.SMS.Net.bd.Fields.EnableOrderPaid.Hint"] = "If enabled, SMS messages will be sent when an order payment is received.",

                ["Plugins.SMS.Net.bd.Fields.OrderPaidSMSFormat"] = "Order Paid SMS Format",
                ["Plugins.SMS.Net.bd.Fields.OrderPaidSMSFormat.Hint"] = "Template for SMS sent when an order is paid.",

                ["Plugins.SMS.Net.bd.Fields.EnabledOrderCanceled"] = "Enable Order Canceled",
                ["Plugins.SMS.Net.bd.Fields.EnabledOrderCanceled.Hint"] = "If enabled, SMS messages will be sent when an order is canceled.",

                ["Plugins.SMS.Net.bd.Fields.OrderCanceledSMSFormat"] = "Order Canceled SMS Format",
                ["Plugins.SMS.Net.bd.Fields.OrderCanceledSMSFormat.Hint"] = "Template for SMS sent when an order is canceled.",

                ["Plugins.SMS.Net.bd.Fields.EnableOrderRefunded"] = "Enable Order Refunded",
                ["Plugins.SMS.Net.bd.Fields.EnableOrderRefunded.Hint"] = "If enabled, SMS messages will be sent when an order is refunded.",

                ["Plugins.SMS.Net.bd.Fields.OrderRefundedSMSFormat"] = "Order Refunded SMS Format",
                ["Plugins.SMS.Net.bd.Fields.OrderRefundedSMSFormat.Hint"] = "Template for SMS sent when an order is refunded.",

                ["Plugins.SMS.Net.bd.Fields.EnabledOrderShipping"] = "Enable Order Shipping",
                ["Plugins.SMS.Net.bd.Fields.EnabledOrderShipping.Hint"] = "If enabled, SMS messages will be sent when an order is shipped.",

                ["Plugins.SMS.Net.bd.Fields.OrderShippingSMSFormat"] = "Order Shipping SMS Format",
                ["Plugins.SMS.Net.bd.Fields.OrderShippingSMSFormat.Hint"] = "Template for SMS sent when an order is shipped. Use tokens like %{ShippingStatus}%.",

                ["Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled"] = "Customer Confirm Order SMS Enable",
                ["Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled.Hint"] = "If enabled, SMS messages will be sent when an order is confirmed.",

                ["Plugins.SMS.Net.bd.Fields.SendToOwnerConfirmOrderSMSEnabled"] = "Owner Confirm Order SMS Enabled",
                ["Plugins.SMS.Net.bd.Fields.SendToOwnerConfirmOrderSMSEnabled.Hint"] = "If enabled, SMS messages will be sent when an order is confirmed.",

                ["Plugins.SMS.Net.bd.Fields.ConfirmOrderSMSForOwnerFormat"] = "Owner Confirm Order SMS Format",
                ["Plugins.SMS.Net.bd.Fields.ConfirmOrderSMSForOwnerFormat.Hint"] = "Template for SMS sent to store owner on order confirmation.",

                ["Plugins.SMS.Net.bd.Fields.TestMessage"] = "Message text",
                ["Plugins.SMS.Net.bd.Fields.TestMessage.Hint"] = "Enter the message text that will be used for sending a test SMS.",

                ["Plugins.SMS.Net.bd.Fields.Number"] = "Number",
                ["Plugins.SMS.Net.bd.Fields.Number.Hint"] = "Enter the recipient phone number for the test SMS."
            });
            //locales
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.TestFailed", "Test message sending failed");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.TestSuccess", "Test message was sent (queued)");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.Fields.Enabled", "Enabled");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.Fields.Enabled.Hint", "Check to enable SMS provider");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.Fields.Email", "Email");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.Fields.Email.Hint", "Alpha email address(e.g. your_phone_number@vtext.com)");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.Fields.TestMessage", "Message text");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.Fields.TestMessage.Hint", "Text of the test message");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.SendTest", "Send");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alpha.SendTest.Hint", "Send test message");

            await base.InstallAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<SmsNetBdSettings>().ConfigureAwait(false);

            //locales
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.TestFailed");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.TestSuccess");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.Fields.Enabled");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.Fields.Enabled.Hint");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.Fields.Email");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.Fields.Email.Hint");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.Fields.TestMessage");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.Fields.TestMessage.Hint");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.SendTest");
            //_localizationService.DeletePluginLocaleResource("Plugins.Sms.Alpha.SendTest.Hint");

            await base.UninstallAsync().ConfigureAwait(false);
        }
    }
}
