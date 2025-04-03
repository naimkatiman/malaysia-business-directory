"use client"

import type React from "react"

import { useState, useEffect } from "react"
import { useSearchParams, useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Slider } from "@/components/ui/slider"
import { Checkbox } from "@/components/ui/checkbox"
import { MapPin, Search, SlidersHorizontal, Loader2 } from "lucide-react"
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet"
import BusinessCard from "@/components/business-card"
import BusinessMap from "@/components/business-map"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"

interface Category {
  id: string
  name: string
  icon: string
}

interface Tag {
  id: string
  name: string
}

interface Business {
  id: string
  name: string
  description: string
  category_id: string
  address: string
  phone: string
  email: string
  website: string
  is_premium: boolean
  is_verified: boolean
  is_claimed: boolean
  rating: number
  review_count: number
  location: any
  categories: Category
  business_images: Array<{
    id: string
    url: string
    is_primary: boolean
  }>
}

export default function SearchPage() {
  const router = useRouter()
  const searchParams = useSearchParams()

  const [searchQuery, setSearchQuery] = useState(searchParams.get("query") || "")
  const [location, setLocation] = useState("Kuala Lumpur")
  const [distance, setDistance] = useState([
    searchParams.get("radius") ? Number.parseInt(searchParams.get("radius")!) / 1000 : 5,
  ])
  const [categoryId, setCategoryId] = useState(searchParams.get("category") || "")
  const [selectedTags, setSelectedTags] = useState<string[]>(searchParams.getAll("tags") || [])
  const [showPremiumOnly, setShowPremiumOnly] = useState(searchParams.get("premium") === "true")
  const [view, setView] = useState("list")
  const [loading, setLoading] = useState(false)
  const [businesses, setBusinesses] = useState<Business[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [tags, setTags] = useState<Tag[]>([])
  const [userLocation, setUserLocation] = useState<{ lat: number; lng: number } | null>(null)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)

  // Fetch categories and tags on component mount
  useEffect(() => {
    const fetchFilters = async () => {
      try {
        // Fetch categories
        const categoriesRes = await fetch("/api/categories")
        const categoriesData = await categoriesRes.json()
        setCategories(categoriesData.categories)

        // Fetch tags
        const tagsRes = await fetch("/api/tags")
        const tagsData = await tagsRes.json()
        setTags(tagsData.tags)
      } catch (error) {
        console.error("Error fetching filters:", error)
      }
    }

    fetchFilters()

    // Get user's location if they allow it
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setUserLocation({
            lat: position.coords.latitude,
            lng: position.coords.longitude,
          })
        },
        (error) => {
          console.error("Error getting location:", error)
        },
      )
    }
  }, [])

  // Fetch businesses based on filters
  useEffect(() => {
    const fetchBusinesses = async () => {
      setLoading(true)
      try {
        let url = `/api/search?page=${page}&limit=10`

        if (searchQuery) url += `&query=${encodeURIComponent(searchQuery)}`
        if (categoryId) url += `&category=${encodeURIComponent(categoryId)}`
        if (showPremiumOnly) url += `&premium=true`
        if (userLocation) {
          url += `&lat=${userLocation.lat}&lng=${userLocation.lng}&radius=${distance[0] * 1000}`
        }
        selectedTags.forEach((tag) => {
          url += `&tags=${encodeURIComponent(tag)}`
        })

        const res = await fetch(url)
        const data = await res.json()

        setBusinesses(data.businesses)
        setTotalPages(data.pagination.pages)
      } catch (error) {
        console.error("Error fetching businesses:", error)
      } finally {
        setLoading(false)
      }
    }

    fetchBusinesses()
  }, [searchQuery, categoryId, selectedTags, showPremiumOnly, distance, userLocation, page])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setPage(1)

    // Update URL with search parameters
    const params = new URLSearchParams()
    if (searchQuery) params.set("query", searchQuery)
    if (categoryId) params.set("category", categoryId)
    if (showPremiumOnly) params.set("premium", "true")
    if (distance[0] !== 5) params.set("radius", (distance[0] * 1000).toString())
    selectedTags.forEach((tag) => params.append("tags", tag))

    router.push(`/search?${params.toString()}`)
  }

  const handleTagChange = (tagId: string, checked: boolean) => {
    if (checked) {
      setSelectedTags([...selectedTags, tagId])
    } else {
      setSelectedTags(selectedTags.filter((id) => id !== tagId))
    }
  }

  const resetFilters = () => {
    setSearchQuery("")
    setCategoryId("")
    setSelectedTags([])
    setShowPremiumOnly(false)
    setDistance([5])
    setPage(1)
    router.push("/search")
  }

  const FilterPanel = () => (
    <div className="space-y-6">
      <div>
        <h3 className="font-medium mb-3">Categories</h3>
        <div className="space-y-2">
          <div className="flex items-center space-x-2">
            <Checkbox id="category-all" checked={categoryId === ""} onCheckedChange={() => setCategoryId("")} />
            <label
              htmlFor="category-all"
              className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
            >
              All Categories
            </label>
          </div>
          {categories.map((category) => (
            <div key={category.id} className="flex items-center space-x-2">
              <Checkbox
                id={`category-${category.id}`}
                checked={categoryId === category.id}
                onCheckedChange={() => setCategoryId(category.id)}
              />
              <label
                htmlFor={`category-${category.id}`}
                className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
              >
                {category.name}
              </label>
            </div>
          ))}
        </div>
      </div>

      <div>
        <h3 className="font-medium mb-3">Tags</h3>
        <div className="space-y-2">
          {tags.map((tag) => (
            <div key={tag.id} className="flex items-center space-x-2">
              <Checkbox
                id={`tag-${tag.id}`}
                checked={selectedTags.includes(tag.id)}
                onCheckedChange={(checked) => handleTagChange(tag.id, checked as boolean)}
              />
              <label
                htmlFor={`tag-${tag.id}`}
                className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
              >
                {tag.name}
              </label>
            </div>
          ))}
        </div>
      </div>

      <div>
        <h3 className="font-medium mb-3">Distance</h3>
        <div className="px-2">
          <Slider defaultValue={[5]} max={50} step={1} value={distance} onValueChange={setDistance} />
          <div className="flex justify-between mt-2 text-sm text-muted-foreground">
            <span>0 km</span>
            <span>{distance[0]} km</span>
            <span>50 km</span>
          </div>
        </div>
      </div>

      <div>
        <h3 className="font-medium mb-3">Business Type</h3>
        <div className="flex items-center space-x-2">
          <Checkbox
            id="premium-only"
            checked={showPremiumOnly}
            onCheckedChange={(checked) => setShowPremiumOnly(checked as boolean)}
          />
          <label
            htmlFor="premium-only"
            className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
          >
            Premium businesses only
          </label>
        </div>
      </div>
    </div>
  )

  return (
    <div className="container py-6">
      <div className="flex flex-col gap-6">
        <div className="flex flex-col gap-4">
          <h1 className="text-3xl font-bold">Search Businesses</h1>

          <form onSubmit={handleSearch} className="flex flex-col sm:flex-row gap-2">
            <div className="relative flex-1">
              <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                type="search"
                placeholder="Search for businesses..."
                className="pl-8"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
            <div className="relative flex-1">
              <MapPin className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                type="text"
                placeholder="Location"
                className="pl-8"
                value={location}
                onChange={(e) => setLocation(e.target.value)}
              />
            </div>
            <Button type="submit">
              <Search className="mr-2 h-4 w-4" />
              Search
            </Button>
          </form>
        </div>

        <div className="flex flex-col md:flex-row gap-6">
          <div className="md:w-1/4 space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-xl font-semibold">Filters</h2>
              <Button variant="ghost" size="sm" className="h-8 px-2 text-xs" onClick={resetFilters}>
                Reset
              </Button>
            </div>

            <div className="md:hidden">
              <Sheet>
                <SheetTrigger asChild>
                  <Button variant="outline" className="w-full">
                    <SlidersHorizontal className="mr-2 h-4 w-4" />
                    Filters
                  </Button>
                </SheetTrigger>
                <SheetContent side="left" className="w-[300px] sm:w-[400px]">
                  <h2 className="text-xl font-semibold mb-6">Filters</h2>
                  <FilterPanel />
                </SheetContent>
              </Sheet>
            </div>

            <div className="hidden md:block">
              <FilterPanel />
            </div>
          </div>

          <div className="md:w-3/4">
            <Tabs value={view} onValueChange={setView}>
              <div className="flex justify-between items-center mb-4">
                <p className="text-sm text-muted-foreground">
                  {loading ? "Searching..." : `${businesses.length} businesses found`}
                </p>
                <TabsList className="grid w-[200px] grid-cols-2">
                  <TabsTrigger value="list">List</TabsTrigger>
                  <TabsTrigger value="map">Map</TabsTrigger>
                </TabsList>
              </div>

              {loading ? (
                <div className="flex justify-center items-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
              ) : (
                <>
                  <TabsContent value="list" className="mt-0">
                    <div className="space-y-4">
                      {businesses.length > 0 ? (
                        <>
                          {businesses.map((business) => (
                            <BusinessCard key={business.id} business={business} />
                          ))}

                          {/* Pagination */}
                          {totalPages > 1 && (
                            <div className="flex justify-center mt-6">
                              <div className="flex space-x-2">
                                <Button
                                  variant="outline"
                                  size="sm"
                                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                                  disabled={page === 1}
                                >
                                  Previous
                                </Button>
                                <div className="flex items-center space-x-1">
                                  {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
                                    <Button
                                      key={p}
                                      variant={p === page ? "default" : "outline"}
                                      size="sm"
                                      className="w-8 h-8 p-0"
                                      onClick={() => setPage(p)}
                                    >
                                      {p}
                                    </Button>
                                  ))}
                                </div>
                                <Button
                                  variant="outline"
                                  size="sm"
                                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                                  disabled={page === totalPages}
                                >
                                  Next
                                </Button>
                              </div>
                            </div>
                          )}
                        </>
                      ) : (
                        <div className="text-center py-12">
                          <p className="text-muted-foreground">No businesses found matching your criteria.</p>
                          <Button variant="link" onClick={resetFilters}>
                            Reset filters
                          </Button>
                        </div>
                      )}
                    </div>
                  </TabsContent>

                  <TabsContent value="map" className="mt-0">
                    <div className="h-[600px] rounded-md overflow-hidden border">
                      <BusinessMap businesses={businesses} userLocation={userLocation} />
                    </div>
                  </TabsContent>
                </>
              )}
            </Tabs>
          </div>
        </div>
      </div>
    </div>
  )
}

