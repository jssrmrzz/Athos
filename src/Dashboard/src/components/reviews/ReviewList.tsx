import { useEffect, useState } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue
} from "@/components/ui/select"
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

    const [searchQuery, setSearchQuery] = useState("")
    const [sentimentFilter, setSentimentFilter] = useState("")
    const [statusFilter, setStatusFilter] = useState("")

    // 🔁 Fetch reviews from API
    const fetchReviews = async () => {
        setLoading(true)
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

    useEffect(() => {
        fetchReviews()
    }, [])

    // ✅ Submit response
    const handleApprove = async (reviewId: string, finalResponse: string) => {
        setSubmittingId(reviewId)

        try {
            const res = await fetch("https://localhost:7157/api/reviews/respond", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ reviewId, finalResponse })
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

    // 🧠 Apply filters and search
    const filteredReviews = reviews.filter(r => {
        const matchesSearch =
            r.comment.toLowerCase().includes(searchQuery.toLowerCase()) ||
            r.suggestedResponse.toLowerCase().includes(searchQuery.toLowerCase())

        const matchesSentiment =
            sentimentFilter === "" || r.sentiment === sentimentFilter

        const matchesStatus =
            statusFilter === "" ||
            (statusFilter === "Responded" && r.status === "Responded") ||
            (statusFilter === "Pending" && r.status !== "Responded")

        return matchesSearch && matchesSentiment && matchesStatus
    })

    return (
        <div className="grid gap-4 px-4">
            {/* 🔎 Search + Filter Controls */}
            <div className="flex flex-wrap gap-2 items-center justify-between">
                <Input
                    placeholder="Search reviews..."
                    value={searchQuery}
                    onChange={e => setSearchQuery(e.target.value)}
                    className="w-full sm:w-1/3"
                />

                <Select
                    value={sentimentFilter}
                    onValueChange={setSentimentFilter}
                >
                    <SelectTrigger className="w-[160px]">
                        <SelectValue placeholder="Sentiment" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="Positive">Positive</SelectItem>
                        <SelectItem value="Neutral">Neutral</SelectItem>
                        <SelectItem value="Negative">Negative</SelectItem>
                    </SelectContent>
                </Select>

                <Select
                    value={statusFilter}
                    onValueChange={setStatusFilter}
                >
                    <SelectTrigger className="w-[160px]">
                        <SelectValue placeholder="Status" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="Pending">Pending</SelectItem>
                        <SelectItem value="Responded">Responded</SelectItem>
                    </SelectContent>
                </Select>

                {/* 🔁 Refresh Button */}
                <Button variant="outline" onClick={fetchReviews} disabled={loading}>
                    {loading ? (
                        <>
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            Refreshing...
                        </>
                    ) : (
                        "🔄 Refresh"
                    )}
                </Button>
            </div>

            {/* 🗂️ Review Cards */}
            {filteredReviews.map(r => {
                const isEditing = editingId === r.reviewId
                const isSubmitting = submittingId === r.reviewId

                return (
                    <Card key={r.reviewId}>
                        <CardContent className="pt-4 space-y-2">
                            <div className="flex justify-between items-center">
                                <h3 className="font-semibold">{r.author}</h3>
                                <Badge
                                    variant={
                                        r.sentiment === "Positive"
                                            ? "default"
                                            : r.sentiment === "Negative"
                                                ? "destructive"
                                                : "secondary"
                                    }
                                >
                                    {r.sentiment}
                                </Badge>
                            </div>

                            <p className="text-sm text-muted-foreground">{r.comment}</p>
                            <p className="text-sm italic">💬 Suggested: {r.suggestedResponse}</p>

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
                                            onClick={() =>
                                                handleApprove(r.reviewId, r.suggestedResponse)
                                            }
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