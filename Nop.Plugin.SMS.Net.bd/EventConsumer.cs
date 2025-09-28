using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Services.Common;
using Nop.Core;
using Nop.Services.Customers;

namespace Nop.Plugin.SMS.Net.bd
{
    public class EventConsumer : IConsumer<OrderPlacedEvent>,
                                 IConsumer<OrderPaidEvent>,
                                 IConsumer<OrderStatusChangedEvent>,
                                 IConsumer<OrderRefundedEvent>,
                                 IConsumer<ShipmentSentEvent>,
                                 IConsumer<ShipmentDeliveredEvent>
    {
        private readonly SmsNetBdSettings _settings;
        private readonly IPluginService _pluginService;
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger<EventConsumer> _logger;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        public EventConsumer(SmsNetBdSettings settings,
            ICustomerService customerService,
            IPluginService pluginService,
            IOrderService orderService,
            IStoreService storeService,
            IStoreContext storeContext,
            ILogger<EventConsumer> logger,
            IAddressService addressService)
        {
            _settings = settings;
            _pluginService = pluginService;
            _orderService = orderService;
            _storeService = storeService;
            _logger = logger;
            _customerService = customerService;
            _storeContext = storeContext;
            _addressService = addressService;

        }

        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
               //is enabled?
            if (!_settings.Enabled)
                return;

            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>("Mobile.sms.net.bd", LoadPluginsMode.All);
            if (pluginDescriptor == null)
                return;
            //if (!_pluginFinder.AuthenticateStore(pluginDescriptor, _storeContext.CurrentStore.Id))
            //    return;

            var plugin = pluginDescriptor.Instance<IPlugin>() as SmsNetBdProvider;
            if (plugin == null)
                return;

            if (_settings.Enabled && _settings.EnabledConfirmOrder)
            {
                var order = eventMessage.Order;
                var customer =  _customerService.GetAddressesByCustomerIdAsync(order.CustomerId).Result.FirstOrDefault();
                //var GetCustomer
                if(customer == null || string.IsNullOrEmpty(customer.PhoneNumber))
                    return; 
                //send SMS
                if (_settings.SendToCustomerConfirmOrderSMSEnabled)
                {
                    string ConfirmOrderSMSFormat = _settings.ConfirmOrderSMSForCustomerFormat;
                    if (ConfirmOrderSMSFormat != null && ConfirmOrderSMSFormat != null)
                    {
                        ConfirmOrderSMSFormat = ConfirmOrderSMSFormat.Replace("%[ID]%", order.Id.ToString());
                        ConfirmOrderSMSFormat = ConfirmOrderSMSFormat.Replace("%[OrderTotal]%", order.OrderTotal.ToString());
                        ConfirmOrderSMSFormat = ConfirmOrderSMSFormat.Replace("%[OwnerPhoneNumber]%", _settings.OwnerNumber);

                    }
                    else
                    {
                        ConfirmOrderSMSFormat = _storeContext.GetCurrentStore().Name + "Order is Placed #" + order.Id.ToString() + " and Total Amount: " + order.OrderTotal.ToString();
                    }
                    if (plugin.SendSmsAsync(customer.PhoneNumber, ConfirmOrderSMSFormat,_settings.sender_id).Result)
                    {
                        //eventMessage.Order.note.Add(new OrderNote
                        //{
                        //    Note = "\"Order placed\" SMS alert (to store owner) has been sent",
                        //    DisplayToCustomer = false,
                        //    CreatedOnUtc = DateTime.UtcNow
                        //});
                        await _orderService.UpdateOrderAsync(order);
                    }
                }
                if (_settings.SendToOwnerConfirmOrderSMSEnabled)
                {
                    string ConfirmOrderSMSFormat = _settings.ConfirmOrderSMSForOwnerFormat;
                    if (ConfirmOrderSMSFormat != null && ConfirmOrderSMSFormat != "")
                    {
                        ConfirmOrderSMSFormat = ConfirmOrderSMSFormat.Replace("%[ID]%", order.Id.ToString());
                        ConfirmOrderSMSFormat = ConfirmOrderSMSFormat.Replace("%[OrderTotal]%", order.OrderTotal.ToString());
                        ConfirmOrderSMSFormat = ConfirmOrderSMSFormat.Replace("%[CustomerPhoneNumber]%", customer.PhoneNumber);
                    }
                    else
                    {
                        ConfirmOrderSMSFormat = _storeContext.GetCurrentStore().Name + " Order is Placed #" + order.Id.ToString() + " and Total Amount: " + order.OrderTotal.ToString();
                    }
                    if (plugin.SendSmsAsync(_settings.OwnerNumber, ConfirmOrderSMSFormat, _settings.sender_id).Result)
                    {
                        //eventMessage.Order.note.Add(new OrderNote
                        //{
                        //    Note = "\"Order placed\" SMS alert (to store owner) has been sent",
                        //    DisplayToCustomer = false,
                        //    CreatedOnUtc = DateTime.UtcNow
                        //});
                        await _orderService.UpdateOrderAsync(order);
                    }
                }
            }
        }

        //public async Task HandleEventAsync(OrderCancelledEvent eventMessage)
        //{
        //    if (!_settings.Enabled || !_settings.EnabledOrderCanceled || !_settings.CustomerEnabled)
        //        return;

        //    var plugin = await LoadPluginAsync().ConfigureAwait(false);
        //    if (plugin is null)
        //        return;

        //    var order = eventMessage.Order;
        //    var address = GetOrderAddress(order);
        //    var storeName = await GetStoreNameAsync(order.StoreId).ConfigureAwait(false);

        //    var template = string.IsNullOrWhiteSpace(_settings.OrderCanceledSMSFormat)
        //        ? $"[{storeName}] Your order #{order.Id} has been cancelled."
        //        : _settings.OrderCanceledSMSFormat;

        //    var message = FormatMessage(template, order, address, storeName);
        //    await TrySendAsync(plugin, address?.PhoneNumber, message).ConfigureAwait(false);
        //}

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            if (!_settings.Enabled || !_settings.EnableOrderPaid)
                return;

            var plugin = await LoadPluginAsync().ConfigureAwait(false);
            if (plugin is null)
                return;

            var order = eventMessage.Order;
            var address = GetOrderAddressAsync(order);
            var storeName = _storeContext.GetCurrentStore().Name;

            var template = string.IsNullOrWhiteSpace(_settings.OrderPaidSMSFormat)
                ? $"[{storeName}] We received your payment for order #{order.Id}."
                : _settings.OrderPaidSMSFormat;

            var message = FormatMessage(template, order, address.Result, storeName);
            await TrySendAsync(plugin, address.Result.PhoneNumber, message).ConfigureAwait(false);
        }

        public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
        {
            if (!_settings.Enabled || !_settings.EnableOrderRefunded)
                return;

            var plugin = await LoadPluginAsync().ConfigureAwait(false);
            if (plugin is null)
                return;

            var order = eventMessage.Order;
            var address = GetOrderAddressAsync(order);
            var storeName = _storeContext.GetCurrentStore().Name;

            var template = string.IsNullOrWhiteSpace(_settings.OrderRefundedSMSFormat)
                ? $"[{storeName}] Order #{order.Id} has been refunded."
                : _settings.OrderRefundedSMSFormat;

            var message = FormatMessage(template, order, address.Result, storeName);
            await TrySendAsync(plugin, address.Result?.PhoneNumber, message).ConfigureAwait(false);
        }

        public async Task HandleEventAsync(ShipmentSentEvent eventMessage)
        {
            if (!_settings.Enabled || !_settings.EnabledOrderShipping)
                return;

            var plugin = await LoadPluginAsync().ConfigureAwait(false);
            if (plugin is null)
                return;

            var shipment = eventMessage.Shipment;
            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId).ConfigureAwait(false);
            if (order == null)
                return;

            var address = GetOrderAddressAsync(order);
            var storeName = _storeContext.GetCurrentStore().Name;

            var template = string.IsNullOrWhiteSpace(_settings.OrderShippingSMSFormat)
                ? $"[{storeName}] Order #{order.Id} has been {order.ShippingStatus}."
                : _settings.OrderShippingSMSFormat;

            var message = FormatMessage(template, order, address.Result, storeName, order.ShippingStatus);
            await TrySendAsync(plugin, address.Result.PhoneNumber, message).ConfigureAwait(false);
        }

        public async Task HandleEventAsync(ShipmentDeliveredEvent eventMessage)
        {
            if (!_settings.Enabled || !_settings.EnabledOrderShipping)
                return;

            var plugin = await LoadPluginAsync().ConfigureAwait(false);
            if (plugin is null)
                return;

            var shipment = eventMessage.Shipment;
            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId).ConfigureAwait(false);
            if (order == null)
                return;

            var address = GetOrderAddressAsync(order);
            var storeName = _storeContext.GetCurrentStore().Name;
          
            var template = string.IsNullOrWhiteSpace(_settings.OrderShippingSMSFormat)
                ? $"[{storeName}] Order #{order.Id} has been {order.ShippingStatus}."
                : _settings.OrderShippingSMSFormat;

            var message = FormatMessage(template, order, address.Result, storeName, order.ShippingStatus);
            await TrySendAsync(plugin, address.Result.PhoneNumber, message).ConfigureAwait(false);
        }
        public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
        {
            if (!_settings.Enabled)
                return;

            var plugin = await LoadPluginAsync().ConfigureAwait(false);
            if (plugin is null)
                return;

            var order = eventMessage.Order;
            if (order == null)
                return;

            var address = await GetOrderAddressAsync(order).ConfigureAwait(false);
            var store = await _storeContext.GetCurrentStoreAsync().ConfigureAwait(false);
            var storeName = store?.Name ?? "Store";

            // Pick message template or fallback
            var template = $"[{storeName}] Your order #{order.Id} status has been updated to {order.OrderStatus}.";

            var message = FormatMessage(template, order, address, storeName);

            if (!string.IsNullOrEmpty(address?.PhoneNumber))
                await TrySendAsync(plugin, address.PhoneNumber, message).ConfigureAwait(false);
        }
        private async Task<SmsNetBdProvider?> LoadPluginAsync()
        {
            try
            {
                // Updated to use GetPluginDescriptorBySystemNameAsync instead of LoadPluginBySystemNameAsync
                var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<SmsNetBdProvider>("Mobile.sms.net.bd", LoadPluginsMode.All).ConfigureAwait(false);
                return pluginDescriptor?.Instance<SmsNetBdProvider>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to load sms.net.bd plugin");
                return null;
            }
        }

        //private async Task<string> GetStoreNameAsync(int storeId)
        //{
        //    var store = await _storeService.GetStoreByIdAsync(storeId).ConfigureAwait(false);
        //    return store?.Name ?? string.Empty;
        //}

        private async Task<Address?> GetOrderAddressAsync(Order order)
        {
            if (order == null)
                return null;

            if (order.ShippingAddressId.HasValue)
                return await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);

            if (order.BillingAddressId > 0)
                return await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            return null;
        }

        private string FormatMessage(string template, Order order, Address? address, string storeName, ShippingStatus? shippingStatus = null)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            var formatted = template;
            formatted = formatted.Replace("%[ID]%", order.Id.ToString(CultureInfo.InvariantCulture));
            formatted = formatted.Replace("%[OrderTotal]%", order.OrderTotal.ToString("F", CultureInfo.InvariantCulture));
            formatted = formatted.Replace("%[OwnerPhoneNumber]%", _settings.OwnerNumber ?? string.Empty);
            formatted = formatted.Replace("%[OrderStatus]%", order.OrderStatus.ToString());
            formatted = formatted.Replace("%[StoreName]%", storeName);
            formatted = formatted.Replace("%[ShippingStatus]%", (shippingStatus ?? order.ShippingStatus).ToString());
            formatted = formatted.Replace("%[CustomerPhoneNumber]%", address?.PhoneNumber ?? string.Empty);
            formatted = formatted.Replace("%[CustomerFirstName]%", address?.FirstName ?? string.Empty);
            return formatted;
        }

        private async Task<bool> TrySendAsync(SmsNetBdProvider plugin, string? phoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(message))
                return false;

            var sent = await plugin.SendSmsAsync(phoneNumber, message, _settings.sender_id).ConfigureAwait(false);
            if (!sent)
                _logger.LogWarning("sms.net.bd message failed for {PhoneNumber}", phoneNumber);

            return sent;
        }

    }
}
