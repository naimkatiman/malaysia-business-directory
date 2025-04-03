import Link from "next/link"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { MapPin, Star, Phone } from "lucide-react"

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
  categories: {
    id: string
    name: string
    icon: string
  }
  business_images: Array<{
    id: string
    url: string
    is_primary: boolean
  }>
}

interface BusinessCardProps {
  business: Business
}

export default function BusinessCard({ business }: BusinessCardProps) {
  // Find primary image or use the first one
  const primaryImage = business.business_images?.find((img) => img.is_primary) ||
    business.business_images?.[0] || { url: "/placeholder.svg?height=200&width=300" }

  // Format the business slug for the URL
  const slug = business.name
    .toLowerCase()
    .replace(/\s+/g, "-")
    .replace(/[^\w-]+/g, "")

  return (
    <Card className="overflow-hidden">
      <CardContent className="p-0">
        <div className="flex flex-col sm:flex-row">
          <div className="sm:w-1/3 h-48 sm:h-auto">
            <img
              src={primaryImage.url || "/placeholder.svg"}
              alt={business.name}
              className="w-full h-full object-cover"
            />
          </div>
          <div className="p-4 sm:w-2/3 flex flex-col">
            <div className="flex justify-between items-start">
              <div>
                <h3 className="font-semibold text-lg">
                  <Link href={`/businesses/${business.id}`} className="hover:underline">
                    {business.name}
                  </Link>
                </h3>
                <div className="flex items-center mt-1">
                  <div className="flex items-center">
                    <Star className="h-4 w-4 fill-amber-500 text-amber-500" />
                    <span className="ml-1 text-sm font-medium">{business.rating.toFixed(1)}</span>
                  </div>
                  <span className="text-xs text-muted-foreground ml-1">({business.review_count} reviews)</span>
                  <span className="mx-2 text-muted-foreground">•</span>
                  <span className="text-sm">{business.categories?.name}</span>
                </div>
              </div>
              <div className="flex gap-1">
                {business.is_premium && <Badge className="bg-amber-500 hover:bg-amber-600">Premium</Badge>}
                {business.is_verified && <Badge className="bg-green-500 hover:bg-green-600">Verified</Badge>}
              </div>
            </div>

            <p className="mt-2 text-sm text-muted-foreground line-clamp-2">{business.description}</p>

            <div className="mt-2 text-sm text-muted-foreground">
              <div className="flex items-start">
                <MapPin className="h-4 w-4 mr-1 mt-0.5 shrink-0" />
                <span>{business.address}</span>
              </div>
              {business.phone && (
                <div className="flex items-start mt-1">
                  <Phone className="h-4 w-4 mr-1 mt-0.5 shrink-0" />
                  <span>{business.phone}</span>
                </div>
              )}
            </div>

            <div className="mt-auto pt-4 flex justify-end">
              <Button asChild variant="outline" size="sm">
                <Link href={`/businesses/${business.id}`}>View Details</Link>
              </Button>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

