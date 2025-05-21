import { Bell, UserCircle } from "lucide-react"
import { DarkModeToggle } from "@/components/ui/DarkModeToggle"

export function Topbar() {
    return (
        <header className="w-full h-16 border-b flex items-center justify-between px-6 bg-white dark:bg-zinc-900">
            <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100">Dashboard</h2>

            <div className="flex items-center gap-4">
                <DarkModeToggle />

                <button className="text-gray-500 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200">
                    <Bell className="h-5 w-5" />
                </button>

                <div className="flex items-center gap-2">
                    <UserCircle className="h-6 w-6 text-gray-600 dark:text-gray-300" />
                    <span className="text-sm text-gray-700 dark:text-gray-200">Business Owner</span>
                </div>
            </div>
        </header>
    )
}