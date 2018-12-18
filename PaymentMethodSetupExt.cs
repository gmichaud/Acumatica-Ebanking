using System;
using System.Collections;
using System.Linq;
using PX.Data;
using PX.Objects.CA;

namespace Velixo.HsbcEbanking
{
    public class PaymentMethodSetupExt : PXGraphExtension<PaymentMethodMaint>
    {
        public enum PaymentMethodType
        {
            Ach,
            Wire,
            Osc
        }

        public override void Initialize()
        {
            EbankingSetup.AddMenuAction(SetupForAch);
            EbankingSetup.AddMenuAction(SetupForWire);
            EbankingSetup.AddMenuAction(SetupForOsc);
        }

        public PXAction<PaymentMethod> EbankingSetup;
        [PXButton(MenuAutoOpen = true)]
        [PXUIField(DisplayName = "Ebanking Setup", MapEnableRights = PXCacheRights.Insert,
            MapViewRights = PXCacheRights.Insert,
            Visibility = PXUIVisibility.Visible)]
        protected IEnumerable ebankingSetup(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<PaymentMethod> SetupForAch;
        [PXButton]
        [PXUIField(DisplayName = "Setup for ACH", MapEnableRights = PXCacheRights.Insert,
            MapViewRights = PXCacheRights.Insert,
            Visibility = PXUIVisibility.Visible)]
        protected void setupForAch()
        {
            CheckPrerequirements();
            AddPaymentMethodDetail(PaymentMethodType.Ach);
            AddRemittanceDetail(PaymentMethodType.Ach);
        }

        public PXAction<PaymentMethod> SetupForWire;
        [PXButton]
        [PXUIField(DisplayName = "Setup for Wire Transfers", MapEnableRights = PXCacheRights.Insert,
            MapViewRights = PXCacheRights.Insert,
            Visibility = PXUIVisibility.Visible)]
        protected void setupForWire()
        {
            CheckPrerequirements();
            AddRemittanceDetail(PaymentMethodType.Wire);
            AddPaymentMethodDetail(PaymentMethodType.Wire);
        }

        public PXAction<PaymentMethod> SetupForOsc;
        [PXButton]
        [PXUIField(DisplayName = "Setup for Outsourced Checks", MapEnableRights = PXCacheRights.Insert,
            MapViewRights = PXCacheRights.Insert,
            Visibility = PXUIVisibility.Visible)]
        protected void setupForOsc()
        {
            CheckPrerequirements();
            AddRemittanceDetail(PaymentMethodType.Osc);
            AddPaymentMethodDetail(PaymentMethodType.Ach);
        }

        private void CheckPrerequirements()
        {
            if (Base.DetailsForCashAccount.Select().Any())
            {
                throw new PXException("This payment method already contains remittance settings. Aborting setup.");
            }

            Base.PaymentMethod.Current.UseForAP = true;
            //Base.PaymentMethod.Current.IntegratedProcessing = true;
            Base.PaymentMethod.Current.APAdditionalProcessing = PaymentMethod.aPAdditionalProcessing.CreateBatchPayment;
            Base.PaymentMethod.Update(Base.PaymentMethod.Current);
        }

        private void AddPaymentMethodDetail(PaymentMethodType type)
        {
            if (type == PaymentMethodType.Osc) return; //Nothing is needed for checks, all is setup in the vendor details

            AddPaymentMethodDetail(1, "CDTRACCTID", "Creditor Account ID", true, "", @"^.{1,34}$");

            if (type == PaymentMethodType.Ach)
            {
                AddPaymentMethodDetail(2, "FININSTID", "Financial Institution ID", true, "", @"^.{1,35}$"); //Ach
            }
            else if (type == PaymentMethodType.Wire)
            {
                AddPaymentMethodDetail(2, "FININSTBIC", "Financial Institution BIC ID", true, "", @"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$"); //Wire
            }

            AddPaymentMethodDetail(3, "FINSTNAME", "Financial Institution Street Name", false, "", @"^.{1,70}$");
            AddPaymentMethodDetail(4, "FINBLDGNB", "Financial Institution Building Number", false, "", @"^.{1,16}$");
            AddPaymentMethodDetail(5, "FINTOWN", "Financial Institution Town", false, "", @"^.{1,35}$");
            AddPaymentMethodDetail(6, "FINSTATE", "Financial Institution State/Province", false, "", @"^.{1,35}$");
            AddPaymentMethodDetail(7, "FINZIP", "Financial Institution Zip/Postal Code", false, "", @"^.{1,16}$");
            AddPaymentMethodDetail(8, "FINCNTRY", "Financial Institution Country", true, "", @"^\w{2}$");
        }

        private void AddRemittanceDetail(PaymentMethodType type)
        {
            AddRemittanceDetail(1, "INITGPTYID", "Initiating Party ID", true, "", @"^.{1,35}$");
            AddRemittanceDetail(2, "DBTRNAME", "Debtor Name", true, "", @"^.{1,140}$");
            AddRemittanceDetail(3, "DBTRSTNAME", "Debtor Street Name", true, "", @"^.{1,70}$");
            AddRemittanceDetail(4, "DBTRBLDGNB", "Debtor Building Number", true, "", @"^.{1,16}$");
            AddRemittanceDetail(5, "DBTRTOWN", "Debtor Town", true, "", @"^.{1,35}$");
            AddRemittanceDetail(6, "DBTRSTATE", "Debtor State/Province", true, "", @"^.{1,35}$");
            AddRemittanceDetail(7, "DBTRZIP", "Debtor Zip/Postal Code", true, "", @"^.{1,16}$");
            AddRemittanceDetail(8, "DBTRCNTRY", "Debtor Country", true, "", @"^\w{2}$");

            if (type == PaymentMethodType.Ach)
            {
                AddRemittanceDetail(9, "DBTRID", "ACH Company ID", true, "", @"^.{1,35}$");
            }

            AddRemittanceDetail(10, "DBTRACCTID", "Debtor Account ID", true, "", @"^.{1,34}$");

            if (type == PaymentMethodType.Ach || type == PaymentMethodType.Osc)
            {
                AddRemittanceDetail(11, "FININSTID", "Financial Institution ID", true, "", @"^.{1,35}$");
            }
            else if (type == PaymentMethodType.Wire)
            {
                AddRemittanceDetail(11, "FININSTBIC", "Financial Institution BIC ID", true, "", @"^.{1,35}$");
            }

            AddRemittanceDetail(12, "FINSTNAME", "Financial Institution Street Name", false, "", @"^.{1,70}$");
            AddRemittanceDetail(13, "FINBLDGNB", "Financial Institution Building Number", false, "", @"^.{1,16}$");
            AddRemittanceDetail(14, "FINTOWN", "Financial Institution Town", false, "", @"^.{1,35}$");
            AddRemittanceDetail(15, "FINSTATE", "Financial Institution State/Province", false, "", @"^.{1,35}$");
            AddRemittanceDetail(16, "FINZIP", "Financial Institution Zip/Postal Code", false, "", @"^.{1,16}$");
            AddRemittanceDetail(17, "FINCNTRY", "Financial Institution Country", true, "", @"^\w{2}$");

            if (type == PaymentMethodType.Osc)
            {
                AddRemittanceDetail(18, "CHQFRMSCD", "Check Form Code", true, "", @"^.{1,35}$");
            }
        }

        private void AddPaymentMethodDetail(short sortOrder, string detailID, string description, bool required, string entryMask, string validationRegExp)
        {
            var newDetail = (PaymentMethodDetail)Base.Details.Cache.CreateInstance();
            newDetail.UseFor = PaymentMethodDetailUsage.UseForVendor;
            newDetail.OrderIndex = sortOrder;
            newDetail.DetailID = detailID;
            newDetail.Descr = description;
            newDetail.IsRequired = required;
            newDetail.EntryMask = entryMask;
            newDetail.ValidRegexp = validationRegExp;
            Base.Details.Insert(newDetail);
        }

        private void AddRemittanceDetail(short sortOrder, string detailID, string description, bool required, string entryMask, string validationRegExp)
        {
            var newDetail = (PaymentMethodDetail)Base.Details.Cache.CreateInstance();
            newDetail.UseFor = PaymentMethodDetailUsage.UseForCashAccount;
            newDetail.OrderIndex = sortOrder;
            newDetail.DetailID = detailID;
            newDetail.Descr = description;
            newDetail.IsRequired = required;
            newDetail.EntryMask = entryMask;
            newDetail.ValidRegexp = validationRegExp;
            Base.Details.Insert(newDetail);
        }
    }
}