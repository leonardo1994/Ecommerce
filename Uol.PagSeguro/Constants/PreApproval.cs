namespace Uol.PagSeguro.Constants.PreApproval
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Charge
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private const string auto = "auto";
        private const string manual = "manual";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Auto
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return auto;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Manual
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return manual;
            }
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Period
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private const string weekly = "WEEKLY";
        private const string monthly = "MONTHLY";
        private const string bimonthly = "BIMONTHLY";
        private const string trimonthly = "TRIMONTHLY";
        private const string semiAnnually = "SEMIANNUALLY";
        private const string yearly = "YEARLY";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Weekly
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return weekly;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Monthly
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return monthly;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Bimonthly
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return bimonthly;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Trimonthly
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return trimonthly;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string SemiAnnually
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return semiAnnually;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Yearly
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return yearly;
            }
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class DayOfWeekMethod
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private const string monday = "MONDAY";
        private const string tuesday = "TUESDAY";
        private const string wednesday = "WEDNESDAY";
        private const string thursday = "THURSDAY";
        private const string friday = "FRIDAY";
        private const string saturday = "SATURDAY";
        private const string sunday = "SUNDAY";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Monday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return monday;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Tuesday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return tuesday;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Wednesday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return wednesday;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Thursday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return thursday;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Friday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return friday;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Saturday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return saturday;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Sunday
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return sunday;
            }
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Status
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private const string initiated = "INITIATED";
        private const string pending = "PENDING";
        private const string active = "ACTIVE";
        private const string cancelled = "CANCELLED";
        private const string cancelledByReceiver = "CANCELLED_BY_RECEIVER";
        private const string cancelledBySender = "CANCELLED_BY_SENDER";
        private const string expired = "EXPIRED";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Pending
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return pending;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Initiated
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return initiated;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Active
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return active;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Cancelled
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return cancelled;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string CancelledByReceiver
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return cancelledByReceiver;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string CancelledBySender
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return cancelledBySender;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string Expired
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            get
            {
                return expired;
            }
        }
    }
}