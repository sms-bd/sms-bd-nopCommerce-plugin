using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Sms.Net.bd.Models;
using Nop.Plugin.SMS.Net.bd;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Orders;

namespace Nop.Plugin.Sms.Net.bd.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public class SmsNetBdController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPluginService _pluginFinder;
        private readonly ISettingService _settingService;
        private readonly SmsNetBdSettings _AlphaSettings;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _senderService;

        public SmsNetBdController(ILocalizationService localizationService,
            IPermissionService permissionService,
            IPluginService pluginFinder,
            ISettingService settingService,
            INotificationService notificationService,
            SmsNetBdSettings AlphaSettings)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._pluginFinder = pluginFinder;
            this._settingService = settingService;
            this._AlphaSettings = AlphaSettings;
            _notificationService = notificationService;

        }
        [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
        public async Task<IActionResult> Configure()
        {

            var model = new SmsNetBdModel
            {
                Enabled = _AlphaSettings.Enabled,
                Email = _AlphaSettings.Email,
                API_Key = _AlphaSettings.API_Key,
                API_Url = string.IsNullOrWhiteSpace(_AlphaSettings.API_Url) ? "https://api.sms.net.bd/sendsms" : _AlphaSettings.API_Url,
                //ConfirmOrderSMSFormat = _AlphaSettings.ConfirmOrderSMSFormat,
                EnabledConfirmOrder = _AlphaSettings.EnabledConfirmOrder,
                EnabledOrderCanceled = _AlphaSettings.EnabledOrderCanceled,
                EnabledOrderCompleted = _AlphaSettings.EnabledOrderCompleted,
                EnabledOrderShipping = _AlphaSettings.EnabledOrderShipping,
                EnabledPaymented = _AlphaSettings.EnabledPaymented,
                EnabledRegistered = _AlphaSettings.EnabledRegistered,
                EnableOrderPaid = _AlphaSettings.EnableOrderPaid,
                OrderCanceledSMSFormat = _AlphaSettings.OrderCanceledSMSFormat,
                OrderCompletedSMSFormat = _AlphaSettings.OrderCompletedSMSFormat,
                OrderShippingSMSFormat = _AlphaSettings.OrderShippingSMSFormat,
                PaymentedSMSFormat = _AlphaSettings.PaymentedSMSFormat,
                RegisteredSMSFormat = _AlphaSettings.RegisteredSMSFormat,
                sender_id = _AlphaSettings.sender_id,
                SendToOwnerConfirmOrderSMSEnabled = _AlphaSettings.SendToOwnerConfirmOrderSMSEnabled,
                SendToCustomerConfirmOrderSMSEnabled = _AlphaSettings.SendToCustomerConfirmOrderSMSEnabled,
                CustomerRegOTPSMSFormat = _AlphaSettings.CustomerRegOTPSMSFormat,
                CustomerRegOTPEnabled = _AlphaSettings.CustomerRegOTPEnabled,
                OwnerNumber = _AlphaSettings.OwnerNumber,
                OwnerEnabled = _AlphaSettings.OwnerEnabled,
                CustomerEnabled = _AlphaSettings.CustomerEnabled,
                SendToOwnerAccRegSMSEnabled = _AlphaSettings.SendToOwnerAccRegSMSEnabled,
                SendToCustomerAccRegSMSEnabled = _AlphaSettings.SendToCustomerAccRegSMSEnabled,
                ConfirmOrderSMSForOwnerFormat = _AlphaSettings.ConfirmOrderSMSForOwnerFormat,
                ConfirmOrderSMSForCustomerFormat = _AlphaSettings.ConfirmOrderSMSForCustomerFormat,
                EnableOrderRefunded = _AlphaSettings.EnableOrderRefunded,
                OrderRefundedSMSFormat = _AlphaSettings.OrderRefundedSMSFormat,
                OrderPaidSMSFormat = _AlphaSettings.OrderPaidSMSFormat,
            };

            return View("~/Plugins/SMS.Net.bd/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
        public async Task<IActionResult> ConfigurePOST(SmsNetBdModel model)
        {

            if (!ModelState.IsValid)
            {
                return await Configure();
            }

            //save settings
            _AlphaSettings.Enabled = model.Enabled;
            _AlphaSettings.Email = model.Email;
            _AlphaSettings.sender_id = model.sender_id;


            _AlphaSettings.CustomerEnabled = model.CustomerEnabled;
            _AlphaSettings.OwnerEnabled = model.OwnerEnabled;
            _AlphaSettings.OwnerNumber = model.OwnerNumber;
            _AlphaSettings.CustomerRegOTPEnabled = model.CustomerRegOTPEnabled;
            _AlphaSettings.CustomerRegOTPSMSFormat = model.CustomerRegOTPSMSFormat;

            _AlphaSettings.OrderCanceledSMSFormat = model.OrderCanceledSMSFormat;
            _AlphaSettings.API_Key = model.API_Key;
            _AlphaSettings.API_Url = model.API_Url;

            // _AlphaSettings.ConfirmOrderSMSFormat = model.ConfirmOrderSMSFormat;
            _AlphaSettings.EnabledConfirmOrder = model.EnabledConfirmOrder;
            _AlphaSettings.SendToCustomerConfirmOrderSMSEnabled = model.SendToCustomerConfirmOrderSMSEnabled;
            _AlphaSettings.SendToOwnerConfirmOrderSMSEnabled = model.SendToOwnerConfirmOrderSMSEnabled;

            _AlphaSettings.EnabledOrderCanceled = model.EnabledOrderCanceled;
            _AlphaSettings.EnabledOrderCompleted = model.EnabledOrderCompleted;
            _AlphaSettings.EnabledOrderShipping = model.EnabledOrderShipping;
            _AlphaSettings.EnabledPaymented = model.EnabledPaymented;
            _AlphaSettings.EnabledRegistered = model.EnabledRegistered;
            _AlphaSettings.EnableOrderRefunded = model.EnableOrderRefunded;
            _AlphaSettings.EnableOrderPaid = model.EnableOrderPaid;
            _AlphaSettings.OrderCompletedSMSFormat = model.OrderCompletedSMSFormat;
            _AlphaSettings.OrderShippingSMSFormat = model.OrderShippingSMSFormat;
            _AlphaSettings.PaymentedSMSFormat = model.PaymentedSMSFormat;
            _AlphaSettings.RegisteredSMSFormat = model.RegisteredSMSFormat;
            _AlphaSettings.ConfirmOrderSMSForCustomerFormat = model.ConfirmOrderSMSForCustomerFormat;
            _AlphaSettings.ConfirmOrderSMSForOwnerFormat = model.ConfirmOrderSMSForOwnerFormat;
            _AlphaSettings.OrderRefundedSMSFormat = model.OrderRefundedSMSFormat;
            _AlphaSettings.OrderPaidSMSFormat = model.OrderPaidSMSFormat;
            await _settingService.SaveSettingAsync(_AlphaSettings).ConfigureAwait(false);

            var savedMessage = await _localizationService.GetResourceAsync("Admin.Plugins.Saved").ConfigureAwait(false);
            _notificationService.SuccessNotification(savedMessage);

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test-sms")]
        [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
        public async Task<IActionResult> TestSms(SmsNetBdModel model)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(model.TestMessage) || string.IsNullOrWhiteSpace(model.Number))
                {
                    //ErrorNotification("Enter test message");
                    _notificationService.ErrorNotification("Enter test message and phone number");
                }
                else
                {
                    var pluginDescriptor = await _pluginFinder.GetPluginDescriptorBySystemNameAsync<IPlugin>("Mobile.sms.net.bd", LoadPluginsMode.All);
                    if (pluginDescriptor == null)
                        throw new InvalidOperationException("Cannot load the plugin.");
                    var plugin = pluginDescriptor.Instance<IPlugin>() as SmsNetBdProvider;
                    if (await plugin.SendSmsAsync(model.Number, model.TestMessage, _AlphaSettings.sender_id).ConfigureAwait(false))
                    {
                        var successMessage = await _localizationService.GetResourceAsync("Plugins.Sms.Net.bd.TestSuccess").ConfigureAwait(false);
                        _notificationService.SuccessNotification(successMessage);

                    }
                    else
                    {

                        var failureMessage = await _localizationService.GetResourceAsync("Plugins.Sms.Net.bd.TestFailed").ConfigureAwait(false);
                        _notificationService.ErrorNotification(failureMessage);
                    }
                }
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.ToString());

            }

            return View("~/Plugins/SMS.Net.bd/Views/Configure.cshtml", model);
        }
    }
}