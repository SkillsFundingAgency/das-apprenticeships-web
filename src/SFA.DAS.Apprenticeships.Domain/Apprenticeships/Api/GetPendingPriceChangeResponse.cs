namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api
{
    public class GetPendingPriceChangeResponse
	{
		public bool HasPendingPriceChange { get; set; }
		public PendingPriceChange PendingPriceChange { get; set; }
	}
}
