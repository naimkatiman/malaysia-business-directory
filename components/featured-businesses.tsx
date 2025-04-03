import Link from "next/link"
import { Card, CardContent, CardFooter } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Star, MapPin, Phone } from "lucide-react"

// This would typically come from the database
const businesses = [
  {
    id: "1",
    name: "Jalan Alor Food Street",
    category: "Restaurants",
    rating: 4.7,
    reviewCount: 1245,
    address: "Jalan Alor, Bukit Bintang, Kuala Lumpur",
    phone: "+60 3-2142 8785",
    isPremium: true,
    image: "/placeholder.svg?height=200&width=300",
    slug: "jalan-alor-food-street",
  },
  {
    id: "2",
    name: "The Coffee Bean & Tea Leaf",
    category: "Cafes",
    rating: 4.5,
    reviewCount: 867,
    address: "Pavilion KL, Bukit Bintang, Kuala Lumpur",
    phone: "+60 3-2141 9620",
    isPremium: true,
    image: "/placeholder.svg?height=200&width=300",
    slug: "coffee-bean-tea-leaf",
  },
  {
    id: "3",
    name: "Suria KLCC",
    category: "Shopping",
    rating: 4.8,
    reviewCount: 2354,
    address: "Kuala Lumpur City Centre, Kuala Lumpur",
    phone: "+60 3-2382 2828",
    isPremium: false,
    image: "/placeholder.svg?height=200&width=300",
    slug: "suria-klcc",
  },
  {
    id: "4",
    name: "Mandarin Oriental",
    category: "Hotels",
    rating: 4.9,
    reviewCount: 1123,
    address: "Kuala Lumpur City Centre, Kuala Lumpur",
    phone: "+60 3-2380 8888",
    isPremium: true,
    image: "/placeholder.svg?height=200&width=300",
    slug: "mandarin-oriental",
  },
]

export default function FeaturedBusinesses() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 w-full max-w-6xl">
      {businesses.map((business) => (
        <Link key={business.id} href={`/businesses/${business.slug}`}>
          <Card className="h-full overflow-hidden hover:shadow-md transition-all">
            <div className="relative">
              <img
                src={business.image || "/placeholder.svg"}
                alt={business.name}
                className="w-full h-40 object-cover"
              />
              {business.isPremium && (
                <Badge className="absolute top-2 right-2 bg-amber-500 hover:bg-amber-600">Premium</Badge>
              )}
            </div>
            <CardContent className="p-4">
              <h3 className="font-semibold truncate">{business.name}</h3>
              <p className="text-sm text-muted-foreground">{business.category}</p>
              <div className="flex items-center mt-2">
                <Star className="h-4 w-4 fill-amber-500 text-amber-500 mr-1" />
                <span className="text-sm font-medium">{business.rating}</span>
                <span className="text-xs text-muted-foreground ml-1">({business.reviewCount} reviews)</span>
              </div>
            </CardContent>
            <CardFooter className="p-4 pt-0 flex flex-col items-start gap-1">
              <div className="flex items-start text-sm text-muted-foreground">
                <MapPin className="h-4 w-4 mr-1 mt-0.5 shrink-0" />
                <span className="line-clamp-1">{business.address}</span>
              </div>
              <div className="flex items-center text-sm text-muted-foreground">
                <Phone className="h-4 w-4 mr-1 shrink-0" />
                <span>{business.phone}</span>
              </div>
            </CardFooter>
          </Card>
        </Link>
      ))}
    </div>
  )
}

