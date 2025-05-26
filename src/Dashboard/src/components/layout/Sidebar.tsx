import { Home, MessageCircle } from "lucide-react";
import { Link } from "react-router-dom";

export function Sidebar() {
    return (
        <aside className="min-w-[10rem] max-w-[12rem] bg-gray-100 border-r p-4 flex flex-col">
            <h1 className="text-xl font-bold mb-6 text-gray-800">Athos</h1>
            <nav className="space-y-4">
                <Link
                    to="/"
                    className="flex items-center gap-2 text-gray-700 hover:text-blue-600 transition-colors"
                >
                    <Home className="h-5 w-5" />
                    <span className="text-sm font-medium">Dashboard</span>
                </Link>
                <Link
                    to="/reviews"
                    className="flex items-center gap-2 text-gray-700 hover:text-blue-600 transition-colors"
                >
                    <MessageCircle className="h-5 w-5" />
                    <span className="text-sm font-medium">Reviews</span>
                </Link>
            </nav>
        </aside>
    );
}