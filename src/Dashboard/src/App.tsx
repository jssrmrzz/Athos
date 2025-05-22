import { Layout } from "./components/layout/Layout";
import { ReviewList } from "./components/reviews/ReviewList";
import { MockApiProvider } from "./context/MockApiContext";
import { MockModeToggle } from "./components/MockModeToggle";
import { MockModeBanner } from "./components/MockModeBanner"

function App() {
    return (
        <MockApiProvider>
            <MockModeBanner />

            <div className="min-h-screen bg-background text-foreground">
                <div className="p-4">
                    <MockModeToggle />
                </div>
                <Layout>
                    <ReviewList />
                </Layout>
            </div>
        </MockApiProvider>
    );
}

export default App;