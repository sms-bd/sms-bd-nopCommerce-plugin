using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel;

namespace Nop.Plugin.Sms.Net.bd.Models
{
    public class SmsNetBdModel
    {
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.Enabled")]
        public bool Enabled { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.CustomerEnabled")]
        public bool CustomerEnabled { get; set; }
        //[NopResourceDisplayName("Plugins.Sms.Alpha.Fields.Email")]
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.ConfirmOrderSMSForOwnerFormat")]
        public string ConfirmOrderSMSForOwnerFormat { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.ConfirmOrderSMSForCustomerFormat")]
        public string ConfirmOrderSMSForCustomerFormat { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.Number")]
        public string Number { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.CustomerRegOTPEnabled")]
        public bool CustomerRegOTPEnabled { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.CustomerRegOTPSMSFormat")]
        public string CustomerRegOTPSMSFormat { get; set; }

        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.API_Url")]
        public string API_Url { get; set; }

        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.API_Key")]
        public string API_Key { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OwnerNumber")]
        public string OwnerNumber { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.sender_id")]
        public string sender_id { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OwnerEnabled")]
        public bool OwnerEnabled { get; set; }
        //registered
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnabledRegistered")]
        public bool EnabledRegistered { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.RegisteredSMSFormat")]
        public string RegisteredSMSFormat { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled")]
        public bool SendToCustomerAccRegSMSEnabled { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled")]
        public bool SendToOwnerAccRegSMSEnabled { get; set; }
        //ConfirmOrder
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled")]
        public bool EnabledConfirmOrder { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled")]
        public string ConfirmOrderSMSFormat { get; set; }

        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.SendToCustomerAccRegSMSEnabled")]
        public bool SendToCustomerConfirmOrderSMSEnabled { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.SendToOwnerConfirmOrderSMSEnabled")]
        public bool SendToOwnerConfirmOrderSMSEnabled { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnableOrderStatusChanged")]
        public bool EnableOrderStatusChanged { get; set; }
        //Paymented
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnabledPaymented")]
        public bool EnabledPaymented { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.PaymentedSMSFormat")]
        public string PaymentedSMSFormat { get; set; }
        //Shipped
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnabledOrderShipping")]
        public bool EnabledOrderShipping { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OrderShippingSMSFormat")]
        public string OrderShippingSMSFormat { get; set; }
        //Completed
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnabledOrderCompleted")]
        public bool EnabledOrderCompleted { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OrderCompletedSMSFormat")]
        public string OrderCompletedSMSFormat { get; set; }
        //Canceled
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnabledOrderCanceled")]
        public bool EnabledOrderCanceled { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OrderCanceledSMSFormat")]
        public string OrderCanceledSMSFormat { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnableOrderRefunded")]
        public bool EnableOrderRefunded { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OrderRefundedSMSFormat")]
        [Description("This is your help text for the nopeditor field.")]
        public string OrderRefundedSMSFormat { get; set; }

        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.EnableOrderPaid")]
        public bool EnableOrderPaid { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.OrderPaidSMSFormat")]
        public string OrderPaidSMSFormat { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.Email")]
        public string Email { get; set; }
        [NopResourceDisplayName("Plugins.SMS.Net.bd.Fields.TestMessage")]
        public string TestMessage { get; set; }
    }
}