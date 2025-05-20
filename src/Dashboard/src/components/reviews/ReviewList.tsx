import { useEffect, useState } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectTrigger,
    SelectValue,
    SelectContent,
    SelectItem,
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

    // 🔍 Search & Filter state
    const [searchQuery, setSearchQuery] = useState("")
    const [sentimentFilter, setSentimentFilter] = useState("")
    const [statusFilter, setStatusFilter] = useState("")

    // 🔄 Fetch reviews on mount
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

    // ✅ Handle approval (or submission of edited reply)
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

    // 💡 Apply all filters and search to review list
    const filteredReviews = reviews.filter(r => {
        const matchesSearch =
            r.comment.toLowerCase().includes(searchQuery.toLowerCase()) ||
            r.suggestedResponse.toLowerCase().includes(searchQuery.toLowerCase())

        const matchesSentiment =
            sentimentFilter === "All" || r.sentiment === sentimentFilter

        const matchesStatus =
            statusFilter === "All" ||
            (statusFilter === "Responded" && r.status === "Responded") ||
            (statusFilter === "Pending" && r.status !== "Responded")

        return matchesSearch && matchesSentiment && matchesStatus
    })

    if (loading) {
        return <p className="text-muted-foreground px-4">Loading reviews...</p>
    }

    return (
        <div className="grid gap-4 px-4">

            {/* 🔍 Search and Filters */}
            <div className="flex flex-col md:flex-row gap-4 md:items-end justify-between mb-4">
                {/* Search input */}
                <div className="flex-1">
                    <label htmlFor="search" className="block text-sm font-medium text-muted-foreground mb-1">
                        Search reviews
                    </label>
                    <Input
                        id="search"
                        type="text"
                        placeholder="e.g. wait time, friendly, rude..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="w-full"
                    />
                </div>

                {/* Sentiment dropdown */}
                <div>
                    <label htmlFor="sentiment" className="block text-sm font-medium text-muted-foreground mb-1">
                        Sentiment
                    </label>
                    <Select
                        value={sentimentFilter}
                        onValueChange={setSentimentFilter}
                    >
                        <SelectTrigger>
                            <SelectValue placeholder="Sentiment" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="All">All</SelectItem> {/* replace "" with "All" */}
                            <SelectItem value="Positive">Positive</SelectItem>
                            <SelectItem value="Neutral">Neutral</SelectItem>
                            <SelectItem value="Negative">Negative</SelectItem>
                        </SelectContent>
                    </Select>
                </div>

                {/* Status dropdown */}
                <div>
                    <label htmlFor="status" className="block text-sm font-medium text-muted-foreground mb-1">
                        Status
                    </label>
                    <Select
                        value={statusFilter}
                        onValueChange={setStatusFilter}
                    >
                        <SelectTrigger>
                            <SelectValue placeholder="Status" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="All">All</SelectItem> {/* replace "" with "All" */}
                            <SelectItem value="Pending">Pending</SelectItem>
                            <SelectItem value="Responded">Responded</SelectItem>
                        </SelectContent>
                    </Select>
                </div>
            </div>

            {filteredReviews.length === 0 && (
                <p className="text-muted-foreground text-sm italic">
                    No reviews match your current search or filters.
                </p>
            )}

            {/* 📋 Review list */}
            {filteredReviews.map(r => {
                const isEditing = editingId === r.reviewId
                const isSubmitting = submittingId === r.reviewId

                return (
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

                            {/* ✏️ Edit mode */}
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

                            {/* ✅ Default actions */}
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