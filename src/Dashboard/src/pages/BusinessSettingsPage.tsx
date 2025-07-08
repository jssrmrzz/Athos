import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { GoogleOAuthButton } from '@/components/GoogleOAuthButton';
import { useGoogleOAuth } from '@/hooks/useGoogleOAuth';
import { toast } from 'sonner';

interface BusinessSettings {
  id: string;
  name: string;
  description?: string;
  preferredLlmProvider?: string;
  defaultResponseTemplate?: string;
  autoRespondToPositiveReviews: boolean;
  enableEmailNotifications: boolean;
  enableSmsNotifications: boolean;
  smsPhoneNumber?: string;
  reviewSyncIntervalMinutes: number;
}

interface BusinessSettingsPageProps {
  businessId: string;
}

export const BusinessSettingsPage: React.FC<BusinessSettingsPageProps> = ({ businessId }) => {
  const [settings, setSettings] = useState<BusinessSettings | null>(null);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  
  const { error: oauthError } = useGoogleOAuth({ businessId });

  useEffect(() => {
    loadSettings();
  }, [businessId]);

  const loadSettings = async () => {
    try {
      setLoading(true);
      // Mock business settings for now
      const mockSettings: BusinessSettings = {
        id: businessId,
        name: "My Business",
        description: "A sample business",
        preferredLlmProvider: "Local",
        defaultResponseTemplate: "Thank you for your review!",
        autoRespondToPositiveReviews: false,
        enableEmailNotifications: true,
        enableSmsNotifications: false,
        smsPhoneNumber: "",
        reviewSyncIntervalMinutes: 30
      };
      setSettings(mockSettings);
    } catch (err) {
      toast.error('Failed to load business settings');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const saveSettings = async () => {
    if (!settings) return;

    try {
      setSaving(true);
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      toast.success('Settings saved successfully');
    } catch (err) {
      toast.error('Failed to save settings');
      console.error(err);
    } finally {
      setSaving(false);
    }
  };

  const handleInputChange = (field: keyof BusinessSettings, value: any) => {
    if (settings) {
      setSettings({ ...settings, [field]: value });
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!settings) {
    return (
      <div className="text-center p-8">
        <p className="text-gray-500">Failed to load business settings</p>
        <Button onClick={loadSettings} className="mt-4">
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Business Settings</h1>
          <p className="text-gray-600">Manage your business configuration and integrations</p>
        </div>
        <Button onClick={saveSettings} disabled={saving}>
          {saving ? 'Saving...' : 'Save Settings'}
        </Button>
      </div>

      <Tabs defaultValue="general" className="w-full">
        <TabsList className="grid w-full grid-cols-4">
          <TabsTrigger value="general">General</TabsTrigger>
          <TabsTrigger value="integrations">Integrations</TabsTrigger>
          <TabsTrigger value="notifications">Notifications</TabsTrigger>
          <TabsTrigger value="automation">Automation</TabsTrigger>
        </TabsList>

        <TabsContent value="general" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Business Information</CardTitle>
              <CardDescription>
                Basic information about your business
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="name">Business Name</Label>
                  <Input
                    id="name"
                    value={settings.name}
                    onChange={(e) => handleInputChange('name', e.target.value)}
                    placeholder="Enter business name"
                  />
                </div>
                <div>
                  <Label htmlFor="description">Description</Label>
                  <Input
                    id="description"
                    value={settings.description || ''}
                    onChange={(e) => handleInputChange('description', e.target.value)}
                    placeholder="Enter business description"
                  />
                </div>
              </div>

              <div>
                <Label htmlFor="syncInterval">Review Sync Interval (minutes)</Label>
                <Input
                  id="syncInterval"
                  type="number"
                  value={settings.reviewSyncIntervalMinutes}
                  onChange={(e) => handleInputChange('reviewSyncIntervalMinutes', parseInt(e.target.value))}
                  min="1"
                  max="1440"
                />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>LLM Configuration</CardTitle>
              <CardDescription>
                Configure your preferred AI provider for response generation
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <Label htmlFor="llmProvider">Preferred LLM Provider</Label>
                <select
                  id="llmProvider"
                  value={settings.preferredLlmProvider || 'Local'}
                  onChange={(e) => handleInputChange('preferredLlmProvider', e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-md"
                >
                  <option value="Local">Local LLM</option>
                  <option value="OpenAI">OpenAI</option>
                </select>
              </div>

              <div>
                <Label htmlFor="responseTemplate">Default Response Template</Label>
                <textarea
                  id="responseTemplate"
                  value={settings.defaultResponseTemplate || ''}
                  onChange={(e) => handleInputChange('defaultResponseTemplate', e.target.value)}
                  placeholder="Enter default response template"
                  rows={4}
                  className="w-full p-2 border border-gray-300 rounded-md"
                />
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="integrations" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Google My Business Integration</CardTitle>
              <CardDescription>
                Connect your Google My Business account to automatically sync reviews
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <GoogleOAuthButton
                  businessId={businessId}
                  onStatusChange={(status) => {
                    console.log('OAuth status changed:', status);
                  }}
                />
                
                {oauthError && (
                  <div className="text-red-500 text-sm bg-red-50 p-3 rounded">
                    Error: {oauthError}
                  </div>
                )}

                <div className="text-sm text-gray-600 space-y-2">
                  <p><strong>What this enables:</strong></p>
                  <ul className="list-disc pl-5 space-y-1">
                    <li>Automatic review sync from Google My Business</li>
                    <li>Real-time review notifications</li>
                    <li>Ability to respond to reviews directly from the dashboard</li>
                  </ul>
                </div>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="notifications" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Notification Settings</CardTitle>
              <CardDescription>
                Configure how you want to be notified about new reviews
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center justify-between">
                <div>
                  <Label htmlFor="emailNotifications">Email Notifications</Label>
                  <p className="text-sm text-gray-600">Receive email alerts for new reviews</p>
                </div>
                <Switch
                  id="emailNotifications"
                  checked={settings.enableEmailNotifications}
                  onCheckedChange={(checked) => handleInputChange('enableEmailNotifications', checked)}
                />
              </div>

              <div className="flex items-center justify-between">
                <div>
                  <Label htmlFor="smsNotifications">SMS Notifications</Label>
                  <p className="text-sm text-gray-600">Receive text messages for new reviews</p>
                </div>
                <Switch
                  id="smsNotifications"
                  checked={settings.enableSmsNotifications}
                  onCheckedChange={(checked) => handleInputChange('enableSmsNotifications', checked)}
                />
              </div>

              {settings.enableSmsNotifications && (
                <div>
                  <Label htmlFor="smsPhone">SMS Phone Number</Label>
                  <Input
                    id="smsPhone"
                    value={settings.smsPhoneNumber || ''}
                    onChange={(e) => handleInputChange('smsPhoneNumber', e.target.value)}
                    placeholder="+1234567890"
                  />
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="automation" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Review Automation</CardTitle>
              <CardDescription>
                Configure automatic responses to reviews
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center justify-between">
                <div>
                  <Label htmlFor="autoRespond">Auto-respond to Positive Reviews</Label>
                  <p className="text-sm text-gray-600">
                    Automatically send responses to 4-5 star reviews
                  </p>
                </div>
                <Switch
                  id="autoRespond"
                  checked={settings.autoRespondToPositiveReviews}
                  onCheckedChange={(checked) => handleInputChange('autoRespondToPositiveReviews', checked)}
                />
              </div>

              <div className="text-sm text-gray-600 bg-blue-50 p-3 rounded">
                <p><strong>Note:</strong> Auto-responses use your default response template and are personalized by AI.</p>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
};