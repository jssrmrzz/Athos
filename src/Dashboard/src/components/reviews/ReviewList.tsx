import { useEffect, useState } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
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
    const [editingId, setEditingId] = useState<string | null>(null)
    const [customResponse, setCustomResponse] = useState<string>("")

    // 🔄 Load reviews from API on mount
    useEffect(() => {
        async function fetchReviews() {
            try {
                const res = await fetch("https://localhost:7157/api/reviews")
                const json = await res.json()
                setReviews(json.data ?? [])
            } catch (err) {
                console.error("❌ Failed to fetch reviews", err)
            } finally {
                setLoading(false)
            }
        }

        fetchReviews()
    }, [])

    // ✅ Submit response (approved or edited)
    const handleApprove = async (reviewId: string, finalResponse: string) => {
        setSubmittingId(reviewId)

        try {
            const res = await fetch("https://localhost:7157/api/reviews/respond", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ reviewId, finalResponse }),
            })

            if (res.ok) {
                setReviews(prev =>
                    prev.map(r =>
                        r.reviewId === reviewId ? { ...r, status: "Responded" } : r
                    )
                )
                setEditingId(null)
            } else {
                console.error("❌ Failed to submit response")
            }
        } catch (err) {
            console.error("⚠️ Error sending response", err)
        } finally {
            setSubmittingId(null)
        }
    }

    if (loading) {
        return <p className="text-muted-foreground px-4">Loading reviews...</p>
    }

    return (
        <div className="grid gap-4 px-4">
            {reviews.map(r => {
                const isEditing = editingId === r.reviewId
                const isSubmitting = submittingId === r.reviewId

                return (
                    <Card key={r.reviewId}>
                        <CardContent className="pt-4 space-y-2">
                            {/* 💬 Reviewer + Sentiment Badge */}
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

                            {/* 📝 Review and AI reply */}
                            <p className="text-sm text-muted-foreground">{r.comment}</p>
                            <p className="text-sm italic">💬 Suggested: {r.suggestedResponse}</p>

                            {/* ✏️ Custom edit mode */}
                            {isEditing && (
                                <div className="space-y-2">
                                    <Textarea
                                        value={customResponse}
                                        onChange={e => setCustomResponse(e.target.value)}
                                        className="text-sm"
                                        rows={4}
                                    />
                                    <div className="flex justify-end gap-2">
                                        <Button
                                            variant="outline"
                                            size="sm"
                                            onClick={() => {
                                                setEditingId(null)
                                                setCustomResponse("")
                                            }}
                                        >
                                            Cancel
                                        </Button>
                                        <Button
                                            size="sm"
                                            onClick={() => handleApprove(r.reviewId, customResponse)}
                                            disabled={isSubmitting}
                                        >
                                            {isSubmitting ? (
                                                <>
                                                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                                    Submitting...
                                                </>
                                            ) : (
                                                "Submit"
                                            )}
                                        </Button>
                                    </div>
                                </div>
                            )}

                            {/* ✅ Status + Actions */}
                            <div className="flex justify-between items-center">
                                <p className="text-xs text-muted-foreground">Status: {r.status}</p>

                                {!isEditing && r.status !== "Responded" && (
                                    <div className="flex gap-2">
                                        <Button
                                            variant="outline"
                                            size="sm"
                                            onClick={() => {
                                                setEditingId(r.reviewId)
                                                setCustomResponse(r.suggestedResponse)
                                            }}
                                        >
                                            Customize
                                        </Button>
                                        <Button
                                            size="sm"
                                            disabled={isSubmitting}
                                            onClick={() => handleApprove(r.reviewId, r.suggestedResponse)}
                                        >
                                            {isSubmitting ? (
                                                <>
                                                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                                    Submitting...
                                                </>
                                            ) : (
                                                "Approve"
                                            )}
                                        </Button>
                                    </div>
                                )}
                            </div>
                        </CardContent>
                    </Card>
                )
            })}
        </div>
    )
}