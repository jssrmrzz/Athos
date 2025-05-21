import { createRoot } from "react-dom/client"
import App from "./App";
import "./index.css";
import { BrowserRouter } from "react-router-dom";

createRoot(document.getElementById("root")!).render(
    <BrowserRouter>
        <div className="min-h-screen bg-background text-foreground antialiased transition-colors duration-300" id="root-theme">
            <App />
        </div>
    </BrowserRouter>
)