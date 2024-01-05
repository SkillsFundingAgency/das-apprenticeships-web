namespace SFA.DAS.Apprenticeships.Web.Exceptions
{
	public class StartUpException : Exception
	{
        public string UiSafeMessage { get; set; }

        /// <summary>
        /// Exception which is thrown when the application fails to start
        /// </summary>
        /// <param name="uiSafeMessage">Warning, this message must be considered safe to display in the UI</param>
        public StartUpException(string uiSafeMessage):base(uiSafeMessage) // At the moment uiSafeMessage is the same as the error message
        {
            UiSafeMessage = uiSafeMessage;
        }
    }
}
