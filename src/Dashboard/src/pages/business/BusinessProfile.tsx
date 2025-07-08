import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { MapPin, Phone, Mail, Globe, Star } from "lucide-react";

export function BusinessProfile() {
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
                    Business Profile
                </h1>
                <p className="text-gray-600 dark:text-gray-400 mt-1">
                    View and manage your business profile information
                </p>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2 space-y-6">
                    <Card>
                        <CardHeader>
                            <CardTitle>Business Overview</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="flex items-start gap-4">
                                <div className="w-16 h-16 bg-gray-300 dark:bg-gray-600 rounded-lg flex items-center justify-center">
                                    <span className="text-lg font-bold text-gray-700 dark:text-gray-200">
                                        MB
                                    </span>
                                </div>
                                <div className="flex-1">
                                    <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
                                        My Business
                                    </h3>
                                    <p className="text-gray-600 dark:text-gray-400">
                                        A professional service business focused on customer satisfaction
                                    </p>
                                    <div className="flex items-center gap-2 mt-2">
                                        <Star className="h-4 w-4 text-yellow-400 fill-current" />
                                        <span className="text-sm font-medium">4.8</span>
                                        <span className="text-sm text-gray-500">• 127 reviews</span>
                                    </div>
                                </div>
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle>Contact Information</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-3">
                            <div className="flex items-center gap-3">
                                <MapPin className="h-4 w-4 text-gray-500" />
                                <span className="text-sm text-gray-700 dark:text-gray-200">
                                    123 Business Street, City, State 12345
                                </span>
                            </div>
                            <div className="flex items-center gap-3">
                                <Phone className="h-4 w-4 text-gray-500" />
                                <span className="text-sm text-gray-700 dark:text-gray-200">
                                    (555) 123-4567
                                </span>
                            </div>
                            <div className="flex items-center gap-3">
                                <Mail className="h-4 w-4 text-gray-500" />
                                <span className="text-sm text-gray-700 dark:text-gray-200">
                                    contact@mybusiness.com
                                </span>
                            </div>
                            <div className="flex items-center gap-3">
                                <Globe className="h-4 w-4 text-gray-500" />
                                <span className="text-sm text-gray-700 dark:text-gray-200">
                                    www.mybusiness.com
                                </span>
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle>Business Hours</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="space-y-2">
                                {[
                                    { day: 'Monday', hours: '9:00 AM - 5:00 PM' },
                                    { day: 'Tuesday', hours: '9:00 AM - 5:00 PM' },
                                    { day: 'Wednesday', hours: '9:00 AM - 5:00 PM' },
                                    { day: 'Thursday', hours: '9:00 AM - 5:00 PM' },
                                    { day: 'Friday', hours: '9:00 AM - 5:00 PM' },
                                    { day: 'Saturday', hours: '10:00 AM - 2:00 PM' },
                                    { day: 'Sunday', hours: 'Closed' }
                                ].map((item) => (
                                    <div key={item.day} className="flex justify-between text-sm">
                                        <span className="text-gray-700 dark:text-gray-200">{item.day}</span>
                                        <span className="text-gray-500 dark:text-gray-400">{item.hours}</span>
                                    </div>
                                ))}
                            </div>
                        </CardContent>
                    </Card>
                </div>

                <div className="space-y-6">
                    <Card>
                        <CardHeader>
                            <CardTitle>Quick Stats</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="text-center">
                                <div className="text-2xl font-bold text-gray-900 dark:text-gray-100">127</div>
                                <div className="text-sm text-gray-600 dark:text-gray-400">Total Reviews</div>
                            </div>
                            <div className="text-center">
                                <div className="text-2xl font-bold text-gray-900 dark:text-gray-100">4.8</div>
                                <div className="text-sm text-gray-600 dark:text-gray-400">Average Rating</div>
                            </div>
                            <div className="text-center">
                                <div className="text-2xl font-bold text-gray-900 dark:text-gray-100">12</div>
                                <div className="text-sm text-gray-600 dark:text-gray-400">This Month</div>
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle>Account Status</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-3">
                            <div className="flex justify-between items-center">
                                <span className="text-sm text-gray-700 dark:text-gray-200">Plan</span>
                                <Badge>Pro</Badge>
                            </div>
                            <div className="flex justify-between items-center">
                                <span className="text-sm text-gray-700 dark:text-gray-200">Status</span>
                                <Badge variant="secondary" className="bg-green-100 text-green-800 dark:bg-green-800 dark:text-green-100">
                                    Active
                                </Badge>
                            </div>
                            <div className="flex justify-between items-center">
                                <span className="text-sm text-gray-700 dark:text-gray-200">Next Billing</span>
                                <span className="text-sm text-gray-500 dark:text-gray-400">Jan 15, 2025</span>
                            </div>
                        </CardContent>
                    </Card>

                    <div className="space-y-2">
                        <Button className="w-full">Edit Profile</Button>
                        <Button variant="outline" className="w-full">View Public Profile</Button>
                    </div>
                </div>
            </div>
        </div>
    );
}