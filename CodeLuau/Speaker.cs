using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
	/// <summary>
	/// Represents a single speaker
	/// </summary>
	public class Speaker
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? Exp { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }
		
		private bool IsEmpty(string field) {
			return string.IsNullOrWhiteSpace(field);
		}

		private bool CheckStandards() {
			var employers = new List<string>() { "Pluralsight", "Microsoft", "Google" };
			var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

			bool meetStandards = Exp > 10 || HasBlog || Certifications.Count() > 3 || employers.Contains(Employer);

			if (!meetStandards) {
				string emailDomain = Email.Split('@').Last();
				if (!domains.Contains(emailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9))) {
					meetStandards = true;
				}
			}

			return meetStandards;
		}

		private bool CheckSessions() {
			var oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
			bool sessionsApproved = true;

			foreach (var session in Sessions) {
				foreach (var tech in oldTechnologies) {
					if (session.Title.Contains(tech) || session.Description.Contains(tech)) {
						session.Approved = false;
						sessionsApproved = false;
						break;
					}
				}
			}
			return sessionsApproved;
		}

		private int CalculateRegistrationFee()
		{
			int fee = 0;
			if (Exp <= 1)
				fee = 500;
			else if (Exp <= 3)
				fee = 250;
			else if (Exp <= 5)
				fee = 100;
			else if (Exp <= 9)
				fee = 50;
			return fee;
		}

		/// <summary>
		/// Register a speaker
		/// </summary>
		/// <returns>speakerID</returns>
		public RegisterResponse Register(IRepository repository)
		{
			try {
				if (IsEmpty(FirstName)) {
					return new RegisterResponse(RegisterError.FirstNameRequired);
				}

				if (IsEmpty(LastName)) {
					return new RegisterResponse(RegisterError.LastNameRequired);
				}

				if (IsEmpty(Email)) {
					return new RegisterResponse(RegisterError.EmailRequired);
				}

				if (!CheckStandards()) {
					return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);
				}
				
				if (Sessions.Count() == 0) {
					return new RegisterResponse(RegisterError.NoSessionsProvided);
				}

				if (!CheckSessions()) {
					return new RegisterResponse(RegisterError.NoSessionsApproved);
				}

				RegistrationFee = CalculateRegistrationFee();
				int? speakerId = repository.SaveSpeaker(this);

				return new RegisterResponse((int)speakerId);
			} catch (Exception) {
				return new RegisterResponse(RegisterError.SomethingWentWrong);
			}
		}
	}
}