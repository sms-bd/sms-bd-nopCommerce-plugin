using Nop.Core.Configuration;
using System.ComponentModel;

namespace Nop.Plugin.SMS.Net.bd
{
    public class SmsNetBdSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the value indicting whether this SMS provider is enabled
        /// </summary>
        public bool Enabled { get; set; }
        public bool CustomerEnabled { get; set; }
        public bool CustomerRegOTPEnabled { get; set; }
        public string CustomerRegOTPSMSFormat { get; set; }

        public bool SendToCustomerAccRegSMSEnabled { get; set; }
        public bool SendToOwnerAccRegSMSEnabled { get; set; }
        /// <summary>
        /// Gets or sets the Alpha email
        /// </summary>
        /// public bool OwnerEnabled { get; set; }
        public string Email { get; set; }
        public bool OwnerEnabled { get; set; }
        public string OwnerNumber { get; set; }
        public string API_Url { get; set; }

        public string API_Key { get; set; }

        public string sender_id { get; set; }

        //registered
        public bool EnabledRegistered { get; set; }
        public string RegisteredSMSFormat { get; set; }
        public bool EnableOrderStatusChanged { get; set; }

        //ConfirmOrder
        public bool EnabledConfirmOrder { get; set; }

        //public string ConfirmOrderSMSFormat { get; set; }
        public bool SendToCustomerConfirmOrderSMSEnabled { get; set; }
        public bool SendToOwnerConfirmOrderSMSEnabled { get; set; }

        //Paymented
        public bool EnabledPaymented { get; set; }

        public string PaymentedSMSFormat { get; set; }

        //Shipped
        public bool EnabledOrderShipping { get; set; }

        public string OrderShippingSMSFormat { get; set; }

        //Completed
        public bool EnabledOrderCompleted { get; set; }

        public string OrderCompletedSMSFormat { get; set; }

        public string ConfirmOrderSMSForOwnerFormat { get; set; }
        public string ConfirmOrderSMSForCustomerFormat { get; set; }
        //Canceled
        public bool EnabledOrderCanceled { get; set; }
        public string OrderCanceledSMSFormat { get; set; }
        public bool EnableOrderRefunded { get; set; }
        public string OrderRefundedSMSFormat { get; set; }
        public bool EnableOrderPaid { get; set; }
        public string OrderPaidSMSFormat { get; set; }


    }
}