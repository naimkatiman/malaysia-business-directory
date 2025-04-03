import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent } from "@/components/ui/card"
import { Search, MapPin, Star, ArrowRight } from "lucide-react"
import { createServerSupabaseClient } from "@/lib/supabase"

async function getFeaturedBusinesses() {
  const supabase = createServerSupabaseClient()

  const { data } = await supabase
    .from("businesses")
    .select(`
      id, 
      name, 
      description, 
      rating, 
      review_count, 
      is_premium,
      categories(name),
      business_images(url, is_primary)
    `)
    .eq("is_premium", true)
    .order("rating", { ascending: false })
    .limit(4)

  return data || []
}

async function getCategories() {
  const supabase = createServerSupabaseClient()

  const { data } = await supabase.from("categories").select("id, name, icon").order("name")

  return data || []
}

export default async function HomePage() {
  const featuredBusinesses = await getFeaturedBusinesses()
  const categories = await getCategories()

  return (
    <div className="flex flex-col min-h-screen">
      {/* Hero Section */}
      <section className="relative bg-gradient-to-r from-primary to-primary/80 text-white py-20">
        <div className="container mx-auto px-4 relative z-10">
          <div className="max-w-3xl mx-auto text-center">
            <h1 className="text-4xl md:text-5xl font-bold mb-6">Discover the Best Businesses in Malaysia</h1>
            <p className="text-lg md:text-xl mb-8 opacity-90">
              Find local businesses, read reviews, and connect with your community
            </p>

            <form action="/search" className="bg-white rounded-lg shadow-lg p-2 flex flex-col md:flex-row gap-2">
              <div className="relative flex-1">
                <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  type="text"
                  name="query"
                  placeholder="Search for restaurants, shops, services..."
                  className="pl-9 bg-transparent border-0 shadow-none focus-visible:ring-0 text-black"
                />
              </div>
              <div className="relative flex-1">
                <MapPin className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  type="text"
                  name="location"
                  placeholder="Kuala Lumpur"
                  className="pl-9 bg-transparent border-0 shadow-none focus-visible:ring-0 text-black"
                />
              </div>
              <Button type="submit" size="lg">
                Search
              </Button>
            </form>
          </div>
        </div>
        <div className="absolute inset-0 bg-black/20 z-0"></div>
      </section>

      {/* Categories Section */}
      <section className="py-16 bg-muted/30">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold mb-4">Browse by Category</h2>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              Explore businesses by category to find exactly what you're looking for
            </p>
          </div>

          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
            {categories.map((category) => (
              <Link key={category.id} href={`/search?category=${category.id}`} className="group">
                <Card className="border-transparent hover:border-primary transition-colors">
                  <CardContent className="p-6 text-center">
                    <div className="w-12 h-12 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4 group-hover:bg-primary/20 transition-colors">
                      <span className="text-2xl">{getCategoryIcon(category.icon)}</span>
                    </div>
                    <h3 className="font-medium">{category.name}</h3>
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Businesses Section */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <div className="flex justify-between items-center mb-12">
            <div>
              <h2 className="text-3xl font-bold mb-2">Featured Businesses</h2>
              <p className="text-muted-foreground">Discover top-rated premium businesses in Malaysia</p>
            </div>
            <Button asChild variant="outline">
              <Link href="/search?premium=true">
                View All <ArrowRight className="ml-2 h-4 w-4" />
              </Link>
            </Button>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {featuredBusinesses.map((business) => {
              // Find primary image or use the first one
              const primaryImage = business.business_images?.find((img) => img.is_primary) ||
                business.business_images?.[0] || { url: "/placeholder.svg?height=200&width=300" }

              // Format the business slug for the URL
              const slug = business.name
                .toLowerCase()
                .replace(/\s+/g, "-")
                .replace(/[^\w-]+/g, "")

              return (
                <Link key={business.id} href={`/businesses/${slug}`}>
                  <Card className="overflow-hidden h-full hover:shadow-md transition-shadow">
                    <div className="aspect-video relative">
                      <img
                        src={primaryImage.url || "/placeholder.svg?height=200&width=300"}
                        alt={business.name}
                        className="w-full h-full object-cover"
                      />
                    </div>
                    <CardContent className="p-4">
                      <h3 className="font-semibold text-lg mb-1 line-clamp-1">{business.name}</h3>
                      <div className="flex items-center mb-2">
                        <Star className="h-4 w-4 fill-amber-500 text-amber-500 mr-1" />
                        <span className="text-sm font-medium">{business.rating.toFixed(1)}</span>
                        <span className="text-xs text-muted-foreground ml-1">({business.review_count} reviews)</span>
                      </div>
                      <p className="text-sm text-muted-foreground line-clamp-2">{business.description}</p>
                    </CardContent>
                  </Card>
                </Link>
              )
            })}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 bg-primary text-white">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl font-bold mb-4">Own a Business?</h2>
            <p className="text-lg mb-8 opacity-90">
              List your business on dekat.me to reach more customers and grow your business
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button size="lg" variant="secondary">
                Add Your Business
              </Button>
              <Button size="lg" variant="outline" className="bg-transparent text-white hover:bg-white/10">
                Learn More
              </Button>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}

// Helper function to render category icons
function getCategoryIcon(iconName: string | null): string {
  const iconMap: Record<string, string> = {
    utensils: "🍽️",
    coffee: "☕",
    "shopping-bag": "🛍️",
    hotel: "🏨",
    car: "🚗",
    briefcase: "💼",
    scissors: "✂️",
    stethoscope: "⚕️",
    film: "🎬",
    book: "📚",
    // Add more icon mappings as needed
  }

  return iconMap[iconName || ""] || "🏢"
}

