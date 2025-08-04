<pre>
 _______  _______  __   __  _______  _______ 
|   _   ||       ||  | |  ||       ||       |
|  |_|  ||_     _||  |_|  ||   _   ||  _____|
|       |  |   |  |       ||  | |  || |_____ 
|       |  |   |  |       ||  |_|  ||_____  |
|   _   |  |   |  |   _   ||       | _____| |
|__| |__|  |___|  |__| |__||_______||_______| 
</pre>

![prescreenTool](images/demo_ATHOS.gif)

🧠 ATHOS – AI-Powered Review Response Automation

ATHOS is a full-stack, production-ready SaaS dashboard designed to help small businesses automate responses to customer reviews across platforms like Google. It uses AI (local or cloud LLMs) to generate thoughtful replies, notify businesses of negative reviews, and streamline reputation management.

🔧 Features
	•	✍️ AI-generated review replies (LLM-integrated)
	•	🛠 Toggle between real and mock API data (sandbox mode)
	•	📱 Responsive dashboard (mobile & desktop)
	•	⭐ Star rating filters for quick triage
	•	🔐 **Production-ready Google OAuth 2.0 integration** 
	•	🏢 **Multi-tenant SaaS architecture** with business isolation
	•	👤 **Real-time user authentication** with profile display
	•	🔄 **Automatic token refresh** and secure disconnection
	•	⚠️ Negative review alerts (coming soon)

🧰 Tech Stack
	•	Frontend: React + TypeScript + Tailwind + shadcn/ui
	•	Backend: .NET 6 Web API (Clean Architecture)
	•	AI: Local model via Ollama (barbershop-rev) or OpenAI
	•	Authentication: Google OAuth 2.0 (server-side flow)
	•	Database: SQLite with multi-tenant business isolation
	•	APIs: Google My Business API integration ready

