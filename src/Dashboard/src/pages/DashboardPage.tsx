import { ReviewList } from "@/components/ReviewList"

export default function DashboardPage() {
    return (
        <div className="p-4">
            <h2 className="text-xl font-bold mb-4">Recent Reviews</h2>
            <ReviewList />
        </div>
    )
}