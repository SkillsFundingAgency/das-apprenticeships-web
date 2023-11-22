namespace SFA.DAS.Apprenticeships.Web.Models
{
    public class DateField
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTime? Date 
        {
            get 
            { 
                if(Day == null || Month == null || Year == null)
                {
                    return null;
                }

                DateTime? date = null;

                try
                {
                    date = new DateTime(Year.Value, Month.Value, Day.Value);
                }
                catch
                {
                    // swallow
                }

                return date;
            } 
        }

    }
}
