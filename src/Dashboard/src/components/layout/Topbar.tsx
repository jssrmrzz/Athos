import { Bell, UserCircle } from "lucide-react";

export function Topbar() {
    return (
        <header className="w-full h-16 bg-white border-b flex items-center justify-between px-6">
            <h2 className="text-lg font-semibold">Dashboard</h2>
            <div className="flex items-center gap-4">
                <button className="text-gray-500 hover:text-gray-800">
                    <Bell className="h-5 w-5" />
                </button>
                <div className="flex items-center gap-2">
                    <UserCircle className="h-6 w-6 text-gray-600" />
                    <span className="text-sm text-gray-700">Business Owner</span>
                </div>
            </div>
        </header>
    );
}