import { Home, MessageCircle } from "lucide-react";
import { Link } from "react-router-dom";

export function Sidebar() {
    return (
        <aside className="w-64 h-screen bg-gray-100 border-r flex flex-col p-4">
            <h1 className="text-xl font-bold mb-6">Athos</h1>
            <nav className="flex flex-col gap-4">
                <Link to="/" className="flex items-center gap-2 text-gray-700 hover:text-blue-600">
                    <Home className="h-5 w-5" />
                    Dashboard
                </Link>
                <Link to="/reviews" className="flex items-center gap-2 text-gray-700 hover:text-blue-600">
                    <MessageCircle className="h-5 w-5" />
                    Reviews
                </Link>
            </nav>
        </aside>
    );
}