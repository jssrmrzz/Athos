import { Layout } from "./components/layout/Layout";
import { ReviewList } from "./components/reviews/ReviewList";

function App() {
    return (
        <div className="min-h-screen bg-background text-foreground">
            <Layout>
                <ReviewList />
            </Layout>
        </div>
    );
}

export default App;