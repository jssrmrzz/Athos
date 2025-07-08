import { ReviewList } from "@/components/reviews/ReviewList"

export default function DashboardPage() {
    return (
        <div>
            <h2 className="text-xl font-bold mb-4">Recent Reviews</h2>
            <ReviewList />
        </div>
    )
}