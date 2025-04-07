import Link from "next/link"
import { Card, CardContent } from "@/components/ui/card"
import { Coffee, ShoppingBag, Utensils, Hotel, Car, Briefcase, Scissors, Stethoscope } from "lucide-react"

// this would typically come from the database
const categories = [
  {
    id: "1",
    name: "Restaurants",
    icon: Utensils,
    count: 1245,
    slug: "restaurants",
  },
  {
    id: "2",
    name: "Cafes",
    icon: Coffee,
    count: 867,
    slug: "cafes",
  },
  {
    id: "3",
    name: "Shopping",
    icon: ShoppingBag,
    count: 1532,
    slug: "shopping",
  },
  {
    id: "4",
    name: "Hotels",
    icon: Hotel,
    count: 423,
    slug: "hotels",
  },
  {
    id: "5",
    name: "Automotive",
    icon: Car,
    count: 678,
    slug: "automotive",
  },
  {
    id: "6",
    name: "Professional Services",
    icon: Briefcase,
    count: 912,
    slug: "professional-services",
  },
  {
    id: "7",
    name: "Beauty & Wellness",
    icon: Scissors,
    count: 745,
    slug: "beauty-wellness",
  },
  {
    id: "8",
    name: "Healthcare",
    icon: Stethoscope,
    count: 534,
    slug: "healthcare",
  },
  {
    id: "9",
    name: "Education",
    icon: Briefcase, // Using Briefcase as a placeholder icon
    count: 328,
    slug: "education",
  },
]

export default function categoryGrid() {
  return (
    <div className="grid grid-cols-2 md:grid-cols-4 gap-4 w-full max-w-4xl">
      {categories.map((category) => (
        <Link key={category.id} href={`/categories/${category.slug}`}>
          <Card className="h-full transition-all hover:bg-muted/50">
            <CardContent className="flex flex-col items-center justify-center p-6">
              <category.icon className="h-8 w-8 mb-2" />
              <h3 className="font-medium text-center">{category.name}</h3>
              <p className="text-xs text-muted-foreground text-center">{category.count} businesses</p>
            </CardContent>
          </Card>
        </Link>
      ))}
    </div>
  )
}

