import type { Metadata } from "next"
import Link from "next/link"
import { notFound } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Globe, Mail, MapPin, Phone, Star, Clock, Share2, Heart, MessageSquare, Edit } from "lucide-react"
import { createServerSupabaseClient } from "@/lib/supabase"
import BusinessReviews from "@/components/business-reviews"
import BusinessMap from "@/components/business-map"

async function getBusinessBySlug(slug: string) {
  const supabase = createServerSupabaseClient()

  // First try to find by slug
  let query = supabase.from("businesses").select(`
      *,
      categories(*),
      business_images(*),
      reviews(*, users(name)),
      tags:business_tags(tags(*))
    `)

  // Check if slug is a UUID
  const isUuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(slug)

  if (isUuid) {
    query = query.eq("id", slug)
  } else {
    // Use ilike for case-insensitive matching on name
    // Convert name to slug format for comparison
    query = query.ilike("name", slug.replace(/-/g, "%"))
  }

  const { data, error } = await query.single()

  if (error || !data) {
    return null
  }

  return data
}

export async function generateMetadata({
  params,
}: {
  params: { slug: string }
}): Promise<Metadata> {
  const business = await getBusinessBySlug(params.slug)

  if (!business) {
    return {
      title: "Business Not Found | dekat.me",
    }
  }

  return {
    title: `${business.name} | dekat.me`,
    description: business.description,
  }
}

export default async function BusinessPage({
  params,
}: {
  params: { slug: string }
}) {
  const business = await getBusinessBySlug(params.slug)

  if (!business) {
    notFound()
  }

  // Get similar businesses
  const supabase = createServerSupabaseClient()
  const { data: similarBusinesses } = await supabase
    .from("businesses")
    .select("id, name, rating, review_count, categories(name), business_images(url, is_primary)")
    .eq("category_id", business.category_id)
    .neq("id", business.id)
    .order("rating", { ascending: false })
    .limit(3)

  return (
    <div className="container py-6 md:py-10">
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-2 space-y-6">
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h1 className="text-3xl font-bold">{business.name}</h1>
                <div className="flex items-center mt-2">
                  <Badge variant="outline">{business.categories?.name}</Badge>
                  {business.is_premium && <Badge className="ml-2 bg-amber-500 hover:bg-amber-600">Premium</Badge>}
                  {business.is_verified && <Badge className="ml-2 bg-green-500 hover:bg-green-600">Verified</Badge>}
                </div>
              </div>
              <div className="flex gap-2">
                <Button variant="outline" size="icon">
                  <Share2 className="h-4 w-4" />
                  <span className="sr-only">Share</span>
                </Button>
                <Button variant="outline" size="icon">
                  <Heart className="h-4 w-4" />
                  <span className="sr-only">Save</span>
                </Button>
              </div>
            </div>

            <div className="flex items-center">
              <Star className="h-5 w-5 fill-amber-500 text-amber-500 mr-1" />
              <span className="text-lg font-medium">{business.rating}</span>
              <span className="text-muted-foreground ml-1">({business.review_count} reviews)</span>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="flex items-start">
                <MapPin className="h-5 w-5 mr-2 mt-0.5 text-muted-foreground shrink-0" />
                <span>{business.address}</span>
              </div>
              {business.phone && (
                <div className="flex items-start">
                  <Phone className="h-5 w-5 mr-2 mt-0.5 text-muted-foreground shrink-0" />
                  <span>{business.phone}</span>
                </div>
              )}
              {business.email && (
                <div className="flex items-start">
                  <Mail className="h-5 w-5 mr-2 mt-0.5 text-muted-foreground shrink-0" />
                  <span>{business.email}</span>
                </div>
              )}
              {business.website && (
                <div className="flex items-start">
                  <Globe className="h-5 w-5 mr-2 mt-0.5 text-muted-foreground shrink-0" />
                  <Link href={business.website} target="_blank" className="hover:underline">
                    {business.website.replace(/^https?:\/\//, "")}
                  </Link>
                </div>
              )}
              {business.operating_hours && (
                <div className="flex items-start">
                  <Clock className="h-5 w-5 mr-2 mt-0.5 text-muted-foreground shrink-0" />
                  <div>
                    <p className="font-medium">Open today</p>
                    <p className="text-sm text-muted-foreground">
                      {business.operating_hours.monday?.open} - {business.operating_hours.monday?.close}
                    </p>
                  </div>
                </div>
              )}
            </div>
          </div>

          <div className="grid grid-cols-3 gap-2">
            {business.business_images?.slice(0, 3).map((image, index) => (
              <div key={index} className="aspect-video overflow-hidden rounded-md">
                <img
                  src={image.url || "/placeholder.svg?height=200&width=300"}
                  alt={`${business.name} - Image ${index + 1}`}
                  className="w-full h-full object-cover"
                />
              </div>
            ))}
          </div>

          <Tabs defaultValue="about">
            <TabsList className="grid w-full grid-cols-3">
              <TabsTrigger value="about">About</TabsTrigger>
              <TabsTrigger value="reviews">Reviews</TabsTrigger>
              <TabsTrigger value="photos">Photos</TabsTrigger>
            </TabsList>
            <TabsContent value="about" className="space-y-4 pt-4">
              <h2 className="text-xl font-semibold">About {business.name}</h2>
              <p>{business.description}</p>

              {business.tags && business.tags.length > 0 && (
                <div className="mt-4">
                  <h3 className="text-lg font-semibold mb-2">Features</h3>
                  <div className="flex flex-wrap gap-2">
                    {business.tags.map((tagObj) => (
                      <Badge key={tagObj.tags.id} variant="outline">
                        {tagObj.tags.name}
                      </Badge>
                    ))}
                  </div>
                </div>
              )}

              <h3 className="text-lg font-semibold mt-6">Opening Hours</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
                {business.operating_hours &&
                  Object.entries(business.operating_hours).map(([day, hours]) => (
                    <div key={day} className="flex justify-between">
                      <span className="capitalize">{day}</span>
                      <span>{hours.open === "closed" ? "Closed" : `${hours.open} - ${hours.close}`}</span>
                    </div>
                  ))}
              </div>
            </TabsContent>
            <TabsContent value="reviews" className="pt-4">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold">Reviews</h2>
                <Button>
                  <MessageSquare className="mr-2 h-4 w-4" />
                  Write a Review
                </Button>
              </div>
              <BusinessReviews reviews={business.reviews || []} />
            </TabsContent>
            <TabsContent value="photos" className="pt-4">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold">Photos</h2>
                <Button variant="outline">
                  <Edit className="mr-2 h-4 w-4" />
                  Add Photos
                </Button>
              </div>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                {business.business_images?.map((image, index) => (
                  <div key={index} className="aspect-square overflow-hidden rounded-md">
                    <img
                      src={image.url || "/placeholder.svg?height=200&width=200"}
                      alt={`${business.name} - Gallery Image ${index + 1}`}
                      className="w-full h-full object-cover"
                    />
                  </div>
                ))}
                {(!business.business_images || business.business_images.length === 0) && (
                  <div className="col-span-4 text-center py-8">
                    <p className="text-muted-foreground">No photos available</p>
                  </div>
                )}
              </div>
            </TabsContent>
          </Tabs>
        </div>

        <div className="space-y-6">
          <div className="rounded-lg border bg-card text-card-foreground shadow-sm p-4">
            <h3 className="font-semibold mb-4">Map Location</h3>
            <div className="aspect-video bg-muted rounded-md overflow-hidden">
              {business.location ? (
                <div className="w-full h-full">
                  <BusinessMap businesses={[business]} userLocation={null} />
                </div>
              ) : (
                <div className="w-full h-full flex items-center justify-center">
                  <MapPin className="h-8 w-8 text-muted-foreground" />
                </div>
              )}
            </div>
            <div className="mt-4">
              <Button className="w-full">Get Directions</Button>
            </div>
          </div>

          {business.is_claimed ? (
            <div className="rounded-lg border bg-card text-card-foreground shadow-sm p-4">
              <h3 className="font-semibold mb-4">Business Owner</h3>
              <p className="text-sm text-muted-foreground mb-4">
                This business has been claimed by the owner or a representative.
              </p>
              <Button variant="outline" className="w-full">
                Contact Business
              </Button>
            </div>
          ) : (
            <div className="rounded-lg border bg-card text-card-foreground shadow-sm p-4">
              <h3 className="font-semibold mb-4">Claim This Business</h3>
              <p className="text-sm text-muted-foreground mb-4">
                Are you the owner or manager of this business? Claim this listing to update information and respond to
                reviews.
              </p>
              <Button className="w-full">Claim Now</Button>
            </div>
          )}

          <div className="rounded-lg border bg-card text-card-foreground shadow-sm p-4">
            <h3 className="font-semibold mb-4">Similar Businesses</h3>
            <div className="space-y-4">
              {similarBusinesses?.map((business) => {
                const primaryImage = business.business_images?.find((img) => img.is_primary) ||
                  business.business_images?.[0] || { url: "/placeholder.svg?height=64&width=64" }

                return (
                  <div key={business.id} className="flex gap-3">
                    <div className="w-16 h-16 rounded-md overflow-hidden shrink-0">
                      <img
                        src={primaryImage.url || "/placeholder.svg"}
                        alt={business.name}
                        className="w-full h-full object-cover"
                      />
                    </div>
                    <div>
                      <h4 className="font-medium">{business.name}</h4>
                      <div className="flex items-center mt-1">
                        <Star className="h-3 w-3 fill-amber-500 text-amber-500 mr-1" />
                        <span className="text-xs">{business.rating}</span>
                        <span className="text-xs text-muted-foreground ml-1">({business.review_count} reviews)</span>
                      </div>
                      <p className="text-xs text-muted-foreground mt-1">{business.categories?.name}</p>
                    </div>
                  </div>
                )
              })}

              {(!similarBusinesses || similarBusinesses.length === 0) && (
                <p className="text-sm text-muted-foreground">No similar businesses found</p>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

