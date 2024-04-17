using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Exceptions;

[ExcludeFromCodeCoverage]
public class StartUpException : Exception
{
        public string UiSafeMessage { get; set; }

        /// <summary>
        /// Exception which is thrown when the application fails to start
        /// </summary>
        /// <param name="uiSafeMessage">Warning, this message must be considered safe to display in the UI</param>
        public StartUpException(string uiSafeMessage):base(uiSafeMessage) 
        {
            UiSafeMessage = uiSafeMessage;
        }

	/// <summary>
	/// Exception which is thrown when the application fails to start
	/// </summary>
	/// <param name="uiSafeMessage">Warning, this message must be considered safe to display in the UI</param>
	/// <param name="exception">The exception which caused the application to fail</param>
	public StartUpException(string uiSafeMessage, Exception exception) : base(exception.Message,exception) 
	{
		UiSafeMessage = uiSafeMessage;
	}
}
