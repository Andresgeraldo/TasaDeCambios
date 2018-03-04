namespace TasaDeCambios.Helpers
{
    using Xamarin.Forms;
    using Interfaces;
    using TasaDeCambios.Properties;

    public static class Lenguages
    {
        static Lenguages()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            TasaDeCambios.Properties.Resources.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string Accept
        {
            get { return Resources.Accept; }
        }

        public static string Title
        {
            get { return Resources.Title; }
        }

        public static string AmountValidation
        {
            get { return Resources.AmountValidation; }
        }

        public static string Error
        {
            get { return Resources.Error; }
        }

        public static string AmountNumericValidation
        {
            get { return Resources.AmountNumericValidation; }
        }

        public static string SourceRateValidation
        {
            get { return Resources.SourceRateValidation; }
        }

        public static string TargetRateValidation
        {
            get { return Resources.TargetRateValidation; }
        }

        public static string AmountLabel
        {
            get { return Resources.AmountLabel; }
        }

        public static string AmountPlaceHolder
        {
            get { return Resources.AmountPlaceHolder; }
        }

        public static string SourceRateLabel
        {
            get { return Resources.SourceRateLabel; }
        }

        public static string SourceRateTitle
        {
            get { return Resources.SourceRateTitle; }
        }

        public static string TargetRateLabel
        {
            get { return Resources.TargetRateLabel; }
        }

        public static string TargetRateTitle
        {
            get { return Resources.TargetRateTitle; }
        }

        public static string Convert
        {
            get { return Resources.Convert; }
        }

        public static string Loading
        {
            get { return Resources.Loading; }
        }

        public static string Ready
        {
            get { return Resources.Ready; }
        }

    }

}
