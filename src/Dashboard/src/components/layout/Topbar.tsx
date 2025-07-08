import { Bell, UserCircle, Settings, User, LogOut } from "lucide-react"
import { DarkModeToggle } from "@/components/ui/DarkModeToggle"
import { DropdownMenu, DropdownMenuItem, DropdownMenuSeparator } from "@/components/ui/dropdown-menu"
import { useNavigate } from "react-router-dom"

export function Topbar() {
    const navigate = useNavigate();

    const handleSettingsClick = () => {
        navigate('/settings');
    };

    const handleProfileClick = () => {
        // Future implementation for profile page
        console.log('Profile clicked');
    };

    const handleSignOutClick = () => {
        // Future implementation for sign out
        console.log('Sign out clicked');
    };

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

                <DropdownMenu 
                    trigger={
                        <div className="flex items-center gap-2 hover:bg-gray-100 dark:hover:bg-gray-800 p-2 rounded-md transition-colors">
                            <UserCircle className="h-6 w-6 text-gray-600 dark:text-gray-300" />
                            <span className="text-sm text-gray-700 dark:text-gray-200 hidden sm:inline">
                                Business Owner
                            </span>
                        </div>
                    }
                >
                    <DropdownMenuItem onClick={handleSettingsClick}>
                        <div className="flex items-center gap-2">
                            <Settings className="h-4 w-4" />
                            Business Settings
                        </div>
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={handleProfileClick}>
                        <div className="flex items-center gap-2">
                            <User className="h-4 w-4" />
                            Profile
                        </div>
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem onClick={handleSignOutClick}>
                        <div className="flex items-center gap-2">
                            <LogOut className="h-4 w-4" />
                            Sign Out
                        </div>
                    </DropdownMenuItem>
                </DropdownMenu>
            </div>
        </header>
    )
}