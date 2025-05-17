import { useEffect, useState } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Loader2 } from "lucide-react"

type Review = {
    reviewId: string
    author: string
    comment: string
    sentiment: string
    suggestedResponse: string
    status: string
}

export function ReviewList() {
    const [reviews, setReviews] = useState<Review[]>([])
    const [loading, setLoading] = useState(true)
    const [submittingId, setSubmittingId] = useState<string | null>(null)

    // Fetch reviews from API
    useEffect(() => {
        async function fetchReviews() {
            try {
                const res = await fetch("https://localhost:7157/api/reviews")
                const json = await res.json()
                setReviews(json.data ?? [])
            } catch (err) {
                console.error("Failed to fetch reviews", err)
            } finally {
                setLoading(false)
            }
        }

        fetchReviews()
    }, [])

    // Approve review via API
    const handleApprove = async (reviewId: string, finalResponse: string) => {
        setSubmittingId(reviewId)

        try {
            const res = await fetch("https://localhost:7157/api/reviews/respond", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ reviewId, finalResponse })
            })

            if (res.ok) {
                setReviews(prev =>
                    prev.map(r =>
                        r.reviewId === reviewId ? { ...r, status: "Responded" } : r
                    )
                )
            } else {
                console.error("❌ Failed to approve review")
            }
        } catch (err) {
            console.error("⚠️ Error sending approval", err)
        } finally {
            setSubmittingId(null)
        }
    }

    if (loading) {
        return <p className="text-muted-foreground px-4">Loading reviews...</p>
    }

    return (
        <div className="grid gap-4 px-4">
            {reviews.map((r) => (
                <Card key={r.reviewId}>
                    <CardContent className="pt-4 space-y-2">
                        <div className="flex justify-between items-center">
                            <h3 className="font-semibold">{r.author}</h3>
                            <Badge variant={
                                r.sentiment === "Positive" ? "default" :
                                    r.sentiment === "Negative" ? "destructive" :
                                        "secondary"
                            }>
                                {r.sentiment}
                            </Badge>
                        </div>

                        <p className="text-sm text-muted-foreground">{r.comment}</p>
                        <p className="text-sm italic">💬 Suggested: {r.suggestedResponse}</p>

                        <div className="flex justify-between items-center">
                            <p className="text-xs text-muted-foreground">Status: {r.status}</p>

                            {r.status !== "Responded" && (
                                <Button
                                    size="sm"
                                    disabled={submittingId === r.reviewId}
                                    onClick={() => handleApprove(r.reviewId, r.suggestedResponse)}
                                >
                                    {submittingId === r.reviewId ? (
                                        <>
                                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                            Submitting...
                                        </>
                                    ) : (
                                        "Approve"
                                    )}
                                </Button>
                            )}
                        </div>
                    </CardContent>
                </Card>
            ))}
        </div>
    )
}