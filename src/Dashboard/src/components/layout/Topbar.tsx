import { Bell, UserCircle } from "lucide-react"
import { DarkModeToggle } from "@/components/ui/DarkModeToggle"
import { BusinessDropdown } from "@/components/business/BusinessDropdown"

export function Topbar() {
    return (
        <header className="w-full h-16 border-b px-4 sm:px-6 bg-white dark:bg-zinc-900 flex items-center justify-between">
            <h2 className="text-base sm:text-lg font-semibold text-gray-800 dark:text-gray-100">
                Dashboard
            </h2>

            <div className="flex items-center gap-4">
                <DarkModeToggle />

                <button
                    className="text-gray-500 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200"
                    aria-label="Notifications"
                >
                    <Bell className="h-5 w-5" />
                </button>

                <div className="flex items-center gap-2">
                    <UserCircle className="h-6 w-6 text-gray-600 dark:text-gray-300" />
                    <BusinessDropdown />
                </div>
            </div>
        </header>
    )
}