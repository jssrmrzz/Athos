import { useState, useEffect, useRef } from "react";
import { ChevronDown, Settings, Users, Building, LogOut, User } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { useOAuthUser } from "@/hooks/useOAuthUser";
import { useToast } from "@/hooks/use-toast";

interface BusinessDropdownProps {
    businessName?: string;
    userRole?: string;
}

export function BusinessDropdown({ 
    businessName = "My Business", 
    userRole = "Business Owner" 
}: BusinessDropdownProps) {
    const [isOpen, setIsOpen] = useState(false);
    const [isSigningOut, setIsSigningOut] = useState(false);
    const navigate = useNavigate();
    const { signOut } = useAuth();
    const { userProfile, isAuthenticated } = useOAuthUser();
    const { toast } = useToast();
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

    const handleSignOutClick = async () => {
        try {
            setIsOpen(false);
            setIsSigningOut(true);
            
            const result = await signOut();
            
            if (result.success) {
                toast({
                    title: "Signed Out",
                    description: "You have been successfully signed out and disconnected from Google.",
                    duration: 3000,
                });
            } else {
                toast({
                    title: "Sign Out Warning",
                    description: result.error || "Sign out completed but there may have been connection issues.",
                    variant: "default",
                    duration: 3000,
                });
            }
        } catch (error) {
            toast({
                title: "Sign Out Error",
                description: "There was an error signing out, but you have been logged out locally.",
                variant: "destructive",
                duration: 3000,
            });
        } finally {
            setIsSigningOut(false);
        }
    };

    // Determine what to display based on authentication status
    const displayName = isAuthenticated && userProfile ? userProfile.name : userRole;

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-200 hover:text-gray-900 dark:hover:text-gray-100 transition-colors"
            >
                {/* Avatar or User Icon */}
                {isAuthenticated && userProfile?.picture ? (
                    <img 
                        src={userProfile.picture} 
                        alt={userProfile.name}
                        className="h-6 w-6 rounded-full object-cover"
                    />
                ) : (
                    <User className="h-4 w-4" />
                )}
                
                {/* Authentication Status Indicator */}
                {isAuthenticated && (
                    <div className="h-2 w-2 bg-green-500 rounded-full"></div>
                )}
                
                <span className="hidden sm:inline">{displayName}</span>
                <ChevronDown className="h-4 w-4" />
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-white dark:bg-zinc-800 rounded-md shadow-lg border border-gray-200 dark:border-zinc-700 z-50">
                    <div className="py-1">
                        <div className="px-3 py-2 border-b border-gray-200 dark:border-zinc-700">
                            {isAuthenticated && userProfile ? (
                                <>
                                    <div className="flex items-center gap-2 mb-1">
                                        <img 
                                            src={userProfile.picture} 
                                            alt={userProfile.name}
                                            className="h-8 w-8 rounded-full object-cover"
                                        />
                                        <div className="flex-1">
                                            <p className="text-sm font-medium text-gray-900 dark:text-gray-100">
                                                {userProfile.name}
                                            </p>
                                            <div className="flex items-center gap-1">
                                                <div className="h-1.5 w-1.5 bg-green-500 rounded-full"></div>
                                                <p className="text-xs text-green-600 dark:text-green-400">
                                                    Authenticated
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <p className="text-xs text-gray-500 dark:text-gray-400">
                                        {userProfile.email}
                                    </p>
                                </>
                            ) : (
                                <>
                                    <p className="text-sm font-medium text-gray-900 dark:text-gray-100">
                                        {businessName}
                                    </p>
                                    <p className="text-xs text-gray-500 dark:text-gray-400">
                                        {userRole} • Not authenticated
                                    </p>
                                </>
                            )}
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
                        
                        <div className="border-t border-gray-200 dark:border-zinc-700 mt-1"></div>
                        
                        <button
                            onClick={handleSignOutClick}
                            disabled={isSigningOut}
                            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-zinc-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            <LogOut className={`h-4 w-4 ${isSigningOut ? 'animate-spin' : ''}`} />
                            {isSigningOut ? 'Signing Out...' : 'Sign Out'}
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}