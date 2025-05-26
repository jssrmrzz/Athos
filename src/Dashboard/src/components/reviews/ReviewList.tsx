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
    SelectValue,
} from "@/components/ui/select"
import { Loader2 } from "lucide-react"
import { useMockApiContext } from "@/context/MockApiContext"
import { useApi } from "@/hooks/useApi"
import { DebugPanel } from "@/components/DebugPanel"
import { showSuccessToast, showErrorToast } from "@/lib/toast"
import { EmptyState } from "@/components/EmptyState"

// Google Review structure-compatible type
type Review = {
    reviewId: string
    reviewer: { displayName: string }
    starRating: number
    comment: string
    suggestedResponse?: string
    finalResponse?: string
    status?: string
    reasoning?: string
}

export function ReviewList() {
    const { useMockApi } = useMockApiContext()
    const api = useApi()
    const [reviews, setReviews] = useState<Review[]>([])
    const [loading, setLoading] = useState(true)
    const [submittingId, setSubmittingId] = useState<string | null>(null)
    const [editingId, setEditingId] = useState<string | null>(null)
    const [customResponse, setCustomResponse] = useState<string>("")

    const [searchQuery, setSearchQuery] = useState("")
    const [statusFilter, setStatusFilter] = useState("all")
    const [ratingFilter, setRatingFilter] = useState("all")
    const [page, setPage] = useState(1)
    const [pageSize, setPageSize] = useState(10)
    const [total, setTotal] = useState(0)
    const totalPages = Math.ceil(total / pageSize)
    const start = (page - 1) * pageSize + 1
    const end = Math.min(page * pageSize, total)
    const [loadingSuggestions, setLoadingSuggestions] = useState<Record<string, boolean>>({})

    const stripThink = (text: string) =>
        text.replace(/<think>.*?<\/think>/gis, "").trim()

    const handleSuggest = async (reviewId: string, author: string, comment: string) => {
        setLoadingSuggestions(prev => ({ ...prev, [reviewId]: true }))
        try {
            const suggestion = await api.suggestResponse({ reviewId, author, comment })
            setReviews(prev =>
                prev.map(r =>
                    r.reviewId === reviewId ? { ...r, suggestedResponse: suggestion } : r
                )
            )
            showSuccessToast("AI Suggestion Ready", "A new response has been generated.")
        } catch {
            showErrorToast("Suggestion Failed", "Unable to generate a response.")
        } finally {
            setLoadingSuggestions(prev => ({ ...prev, [reviewId]: false }))
        }
    }

    const mapStarRating = (input: string | number): number => {
        const map: Record<string, number> = {
            ONE: 1,
            TWO: 2,
            THREE: 3,
            FOUR: 4,
            FIVE: 5,
        }

        if (typeof input === "string") {
            return map[input.toUpperCase()] || 0
        }

        return input
    }

    const fetchReviews = async () => {
        setLoading(true)
        try {
            const { data, total } = await api.getReviews(page, pageSize)

            const normalized = data.map((r: any) => ({
                ...r,
                starRating: mapStarRating(r.starRating)
            }))

            setReviews(normalized)
            setTotal(total)
        } catch (err) {
            showErrorToast("Failed to Load Reviews", "Please check your network or try again.")
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        fetchReviews()
    }, [useMockApi, page, pageSize])

    const handleApprove = async (reviewId: string, finalResponse: string) => {
        setSubmittingId(reviewId)
        try {
            await api.postResponse(reviewId, finalResponse)
            setReviews(prev =>
                prev.map(r =>
                    r.reviewId === reviewId ? { ...r, status: "Responded" } : r
                )
            )
            setEditingId(null)
            showSuccessToast("Response Submitted", "Your reply was saved successfully.")
        } catch {
            showErrorToast("Submission Failed", "We couldn't send your response. Try again.")
        } finally {
            setSubmittingId(null)
        }
    }

    const filteredReviews = reviews.filter(r => {
        const matchesSearch =
            r.comment.toLowerCase().includes(searchQuery.toLowerCase()) ||
            r.suggestedResponse?.toLowerCase().includes(searchQuery.toLowerCase())

        const matchesStatus =
            statusFilter === "all" ||
            (statusFilter === "Responded" && r.status === "Responded") ||
            (statusFilter === "Pending" && r.status !== "Responded")

        const matchesRating =
            ratingFilter === "all" ||
            (ratingFilter === "positive" && r.starRating >= 4) ||
            (ratingFilter === "neutral" && r.starRating === 3) ||
            (ratingFilter === "negative" && r.starRating <= 2)

        return matchesSearch && matchesStatus && matchesRating
    })

    return (
        <div className="grid gap-4 px-4">
            <div className="flex flex-col sm:flex-row sm:flex-wrap gap-2 sm:items-center sm:justify-between">
                <Input
                    placeholder="Search reviews..."
                    value={searchQuery}
                    onChange={e => {
                        setSearchQuery(e.target.value)
                        setPage(1)
                    }}
                    className="w-full sm:w-1/3"
                />

                <Select
                    value={statusFilter}
                    onValueChange={(value) => {
                        setStatusFilter(value)
                        setPage(1)
                    }}
                >
                    <SelectTrigger className="w-[160px]">
                        <SelectValue placeholder="All Statuses" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">All</SelectItem>
                        <SelectItem value="Pending">Pending</SelectItem>
                        <SelectItem value="Responded">Responded</SelectItem>
                    </SelectContent>
                </Select>

                <Select
                    value={ratingFilter}
                    onValueChange={(value) => {
                        setRatingFilter(value)
                        setPage(1)
                    }}
                >
                    <SelectTrigger className="w-[160px]">
                        <SelectValue placeholder="All Ratings" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">All Ratings</SelectItem>
                        <SelectItem value="positive">⭐️ 4–5 Stars (Positive)</SelectItem>
                        <SelectItem value="neutral">⭐️ 3 Stars (Neutral)</SelectItem>
                        <SelectItem value="negative">⚠️ 1–2 Stars (Negative)</SelectItem>
                    </SelectContent>
                </Select>

                <Button variant="outline" onClick={fetchReviews} disabled={loading}>
                    {loading ? (
                        <><Loader2 className="mr-2 h-4 w-4 animate-spin" />Refreshing...</>
                    ) : "🔄 Refresh"}
                </Button>

                <Select value={String(pageSize)} onValueChange={(v) => {
                    setPageSize(Number(v))
                    setPage(1)
                }}>
                    <SelectTrigger className="w-[120px]">
                        <SelectValue placeholder="Page size" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="5">5 per page</SelectItem>
                        <SelectItem value="10">10 per page</SelectItem>
                        <SelectItem value="20">20 per page</SelectItem>
                        <SelectItem value="50">50 per page</SelectItem>
                    </SelectContent>
                </Select>
            </div>

            {filteredReviews.map(r => {
                const isEditing = editingId === r.reviewId
                const isSubmitting = submittingId === r.reviewId
                const match = r.suggestedResponse?.match(/<think>(.*?)<\/think>/is)
                const reasoning = match?.[1]?.trim()
                const message = r.suggestedResponse?.replace(/<think>.*?<\/think>/gis, "").trim()

                const starColor = r.starRating >= 4
                    ? "bg-green-100 text-green-800"
                    : r.starRating === 3
                        ? "bg-yellow-100 text-yellow-800"
                        : "bg-red-100 text-red-800"

                return (
                    <Card key={r.reviewId}>
                        <CardContent className="pt-4 space-y-2">
                            <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-1">
                                <h3 className="font-semibold">{r.reviewer.displayName}</h3>
                                <div className="flex gap-2 flex-wrap items-center">
                                    <span className={`text-xs font-medium rounded-full px-2 py-1 pointer-events-none cursor-default ${starColor}`}>
                                        ⭐ {r.starRating} Star{r.starRating > 1 ? "s" : ""}
                                    </span>
                                    <Badge className="text-xs w-fit" variant={r.status === "Responded" ? "default" : "destructive"}>{r.status}</Badge>
                                </div>
                            </div>

                            <p className="text-sm text-muted-foreground whitespace-pre-wrap">{r.comment}</p>

                            {r.suggestedResponse && (
                                <div className="space-y-1">
                                    <p className="text-sm italic whitespace-pre-wrap break-words">
                                        💬 Suggested: {message}
                                    </p>
                                    {reasoning && (
                                        <p className="text-xs text-muted-foreground italic border-l-2 pl-2 border-muted whitespace-pre-wrap">
                                            🧠 <span className="font-medium">AI Reasoning Flow:</span> {reasoning}
                                        </p>
                                    )}
                                </div>
                            )}

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
                                        >Cancel</Button>
                                        <Button
                                            size="sm"
                                            onClick={() => handleApprove(r.reviewId, stripThink(customResponse))}
                                            disabled={isSubmitting}
                                        >
                                            {isSubmitting ? <><Loader2 className="mr-2 h-4 w-4 animate-spin" />Submitting...</> : "Submit"}
                                        </Button>
                                    </div>
                                </div>
                            )}

                            {!isEditing && r.status !== "Responded" && (
                                <div className="flex flex-wrap gap-2">
                                    <Button
                                        variant="ghost"
                                        size="sm"
                                        onClick={() => handleSuggest(r.reviewId, r.reviewer.displayName, r.comment)}
                                        disabled={loadingSuggestions[r.reviewId] || isSubmitting}
                                    >
                                        {loadingSuggestions[r.reviewId] ? <Loader2 className="w-4 h-4 animate-spin" /> : "💡 Suggest"}
                                    </Button>
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        onClick={() => {
                                            setEditingId(r.reviewId)
                                            setCustomResponse(r.suggestedResponse || "")
                                        }}
                                        disabled={isSubmitting}
                                    >Customize</Button>
                                    <Button
                                        size="sm"
                                        onClick={() => handleApprove(r.reviewId, r.suggestedResponse || "")}
                                        disabled={isSubmitting}
                                    >
                                        {isSubmitting ? <><Loader2 className="mr-2 h-4 w-4 animate-spin" />Submitting...</> : "Approve"}
                                    </Button>
                                </div>
                            )}
                        </CardContent>
                    </Card>
                )
            })}

            {filteredReviews.length === 0 && (
                <EmptyState
                    message="No reviews match your filters."
                    actionText="Clear filters"
                    onAction={() => {
                        setSearchQuery("")
                        setStatusFilter("all")
                        setRatingFilter("all")
                    }}
                />
            )}

            {filteredReviews.length > 0 && (
                <>
                    <p className="text-sm text-muted-foreground text-center mt-2">
                        Showing {start}–{end} of {total} reviews
                    </p>
                    <div className="flex flex-col sm:flex-row items-center justify-center gap-2 sm:gap-4 mt-4 text-center">
                        <Button
                            variant="outline"
                            size="sm"
                            disabled={page === 1 || loading}
                            onClick={() => setPage(prev => Math.max(prev - 1, 1))}
                        >⬅️ Prev</Button>
                        <p className="text-sm text-muted-foreground">Page {page} of {totalPages}</p>
                        <Button
                            variant="outline"
                            size="sm"
                            disabled={page >= totalPages || loading}
                            onClick={() => setPage(prev => prev + 1)}
                        >Next ➡️</Button>
                    </div>
                </>
            )}

            <DebugPanel onRefresh={fetchReviews} />
        </div>
    )
}