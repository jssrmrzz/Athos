import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";

export function BusinessSettings() {
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
                    Business Settings
                </h1>
                <p className="text-gray-600 dark:text-gray-400 mt-1">
                    Manage your business information and preferences
                </p>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Business Information</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="businessName">Business Name</Label>
                            <Input
                                id="businessName"
                                placeholder="Enter business name"
                                defaultValue="My Business"
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="businessEmail">Business Email</Label>
                            <Input
                                id="businessEmail"
                                type="email"
                                placeholder="Enter business email"
                                defaultValue="business@example.com"
                            />
                        </div>
                    </div>
                    
                    <div className="space-y-2">
                        <Label htmlFor="businessDescription">Business Description</Label>
                        <Textarea
                            id="businessDescription"
                            placeholder="Describe your business..."
                            defaultValue="A description of my business"
                        />
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Review Settings</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="flex items-center justify-between">
                        <div className="space-y-0.5">
                            <Label>Auto-generate responses</Label>
                            <p className="text-sm text-gray-600 dark:text-gray-400">
                                Automatically generate AI responses for new reviews
                            </p>
                        </div>
                        <Switch defaultChecked />
                    </div>
                    
                    <div className="flex items-center justify-between">
                        <div className="space-y-0.5">
                            <Label>SMS notifications</Label>
                            <p className="text-sm text-gray-600 dark:text-gray-400">
                                Send SMS alerts for new reviews
                            </p>
                        </div>
                        <Switch />
                    </div>
                </CardContent>
            </Card>

            <div className="flex justify-end">
                <Button>Save Changes</Button>
            </div>
        </div>
    );
}