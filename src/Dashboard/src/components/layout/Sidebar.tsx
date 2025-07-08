import { Home, MessageCircle } from "lucide-react";
import { Link } from "react-router-dom";

export function Sidebar() {
    return (
        <aside className="min-w-[10rem] max-w-[12rem] bg-gray-100 dark:bg-zinc-900 border-r border-gray-200 dark:border-zinc-700 p-4 flex flex-col">
            <h1 className="text-xl font-bold mb-6 text-gray-800 dark:text-gray-100">Athos</h1>
            <nav className="space-y-4">
                <Link
                    to="/"
                    className="flex items-center gap-2 text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                    <Home className="h-5 w-5" />
                    <span className="text-sm font-medium">Dashboard</span>
                </Link>
                <Link
                    to="/reviews"
                    className="flex items-center gap-2 text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                    <MessageCircle className="h-5 w-5" />
                    <span className="text-sm font-medium">Reviews</span>
                </Link>
            </nav>
        </aside>
    );
}