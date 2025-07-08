import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { UserPlus, MoreHorizontal } from "lucide-react";

export function BusinessUsers() {
    const users = [
        {
            id: 1,
            name: "John Doe",
            email: "john@example.com",
            role: "Owner",
            status: "Active"
        },
        {
            id: 2,
            name: "Jane Smith",
            email: "jane@example.com",
            role: "Admin",
            status: "Active"
        },
        {
            id: 3,
            name: "Bob Johnson",
            email: "bob@example.com",
            role: "Manager",
            status: "Pending"
        }
    ];

    const getRoleBadgeColor = (role: string) => {
        switch (role) {
            case "Owner": return "bg-purple-100 text-purple-800 dark:bg-purple-800 dark:text-purple-100";
            case "Admin": return "bg-blue-100 text-blue-800 dark:bg-blue-800 dark:text-blue-100";
            case "Manager": return "bg-green-100 text-green-800 dark:bg-green-800 dark:text-green-100";
            default: return "bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-100";
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
                        Team Members
                    </h1>
                    <p className="text-gray-600 dark:text-gray-400 mt-1">
                        Manage your business team and their permissions
                    </p>
                </div>
                <Button>
                    <UserPlus className="h-4 w-4 mr-2" />
                    Invite User
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Invite New User</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="flex gap-2">
                        <Input 
                            placeholder="Enter email address"
                            className="flex-1"
                        />
                        <select className="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-zinc-800">
                            <option value="viewer">Viewer</option>
                            <option value="manager">Manager</option>
                            <option value="admin">Admin</option>
                        </select>
                        <Button>Send Invite</Button>
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Current Team Members</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="space-y-4">
                        {users.map((user) => (
                            <div key={user.id} className="flex items-center justify-between p-3 bg-gray-50 dark:bg-zinc-800 rounded-lg">
                                <div className="flex items-center gap-3">
                                    <div className="w-10 h-10 bg-gray-300 dark:bg-gray-600 rounded-full flex items-center justify-center">
                                        <span className="text-sm font-medium text-gray-700 dark:text-gray-200">
                                            {user.name.split(' ').map(n => n[0]).join('')}
                                        </span>
                                    </div>
                                    <div>
                                        <p className="font-medium text-gray-900 dark:text-gray-100">
                                            {user.name}
                                        </p>
                                        <p className="text-sm text-gray-600 dark:text-gray-400">
                                            {user.email}
                                        </p>
                                    </div>
                                </div>
                                <div className="flex items-center gap-2">
                                    <Badge className={getRoleBadgeColor(user.role)}>
                                        {user.role}
                                    </Badge>
                                    <Badge variant={user.status === 'Active' ? 'default' : 'secondary'}>
                                        {user.status}
                                    </Badge>
                                    <Button variant="ghost" size="sm">
                                        <MoreHorizontal className="h-4 w-4" />
                                    </Button>
                                </div>
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        </div>
    );
}