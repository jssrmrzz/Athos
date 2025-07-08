import { useState, useEffect, useRef } from "react";
import { ChevronDown, Settings, Users, Building } from "lucide-react";
import { useNavigate } from "react-router-dom";

interface BusinessDropdownProps {
    businessName?: string;
    userRole?: string;
}

export function BusinessDropdown({ 
    businessName = "My Business", 
    userRole = "Business Owner" 
}: BusinessDropdownProps) {
    const [isOpen, setIsOpen] = useState(false);
    const navigate = useNavigate();
    const dropdownRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleMenuClick = (path: string) => {
        navigate(path);
        setIsOpen(false);
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-200 hover:text-gray-900 dark:hover:text-gray-100 transition-colors"
            >
                <span className="hidden sm:inline">{userRole}</span>
                <ChevronDown className="h-4 w-4" />
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-white dark:bg-zinc-800 rounded-md shadow-lg border border-gray-200 dark:border-zinc-700 z-50">
                    <div className="py-1">
                        <div className="px-3 py-2 border-b border-gray-200 dark:border-zinc-700">
                            <p className="text-sm font-medium text-gray-900 dark:text-gray-100">
                                {businessName}
                            </p>
                            <p className="text-xs text-gray-500 dark:text-gray-400">
                                {userRole}
                            </p>
                        </div>
                        
                        <button
                            onClick={() => handleMenuClick('/business/settings')}
                            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-zinc-700 transition-colors"
                        >
                            <Settings className="h-4 w-4" />
                            Business Settings
                        </button>
                        
                        <button
                            onClick={() => handleMenuClick('/business/users')}
                            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-zinc-700 transition-colors"
                        >
                            <Users className="h-4 w-4" />
                            Manage Users
                        </button>
                        
                        <button
                            onClick={() => handleMenuClick('/business/profile')}
                            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-zinc-700 transition-colors"
                        >
                            <Building className="h-4 w-4" />
                            Business Profile
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}