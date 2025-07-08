import { Routes, Route } from "react-router-dom";
import { Layout } from "./components/layout/Layout";

import { ReviewList } from "./components/reviews/ReviewList";
import { BusinessSettings } from "./pages/business/BusinessSettings";
import { BusinessUsers } from "./pages/business/BusinessUsers";
import { BusinessProfile } from "./pages/business/BusinessProfile";


import { MockApiProvider } from "./context/MockApiContext";
import { MockModeToggle } from "./components/MockModeToggle";
import { MockModeBanner } from "./components/MockModeBanner"
import { Toaster } from "@/components/ui/toaster"
import { BusinessSettingsPage } from "./pages/BusinessSettingsPage";
import DashboardPage from "./pages/DashboardPage";

function App() {
    return (
        <MockApiProvider>
            <MockModeBanner />
            <Toaster />

            <div className="min-h-screen bg-background text-foreground">
                <div className="p-4">
                    <MockModeToggle />
                </div>
                <Layout>
                    <Routes>

                        <Route path="/" element={<ReviewList />} />
                        <Route path="/reviews" element={<ReviewList />} />
                        <Route path="/business/settings" element={<BusinessSettings />} />
                        <Route path="/business/users" element={<BusinessUsers />} />
                        <Route path="/business/profile" element={<BusinessProfile />} />

                        <Route path="/" element={<DashboardPage />} />
                        <Route path="/settings" element={<BusinessSettingsPage businessId="1" />} />

                    </Routes>
                </Layout>
            </div>
        </MockApiProvider>
    );
}

export default App;