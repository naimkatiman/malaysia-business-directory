"use client"

import type React from "react"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { supabase } from "@/lib/supabase"
import { Loader2 } from "lucide-react"

// Mock categories for the form
const categories = [
  { id: "1", name: "Restaurants" },
  { id: "2", name: "Cafes" },
  { id: "3", name: "Shopping" },
  { id: "4", name: "Hotels" },
  { id: "5", name: "Automotive" },
  { id: "6", name: "Professional Services" },
  { id: "7", name: "Beauty & Wellness" },
  { id: "8", name: "Healthcare" },
]

export default function AddBusinessPage() {
  const router = useRouter()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  const [formData, setFormData] = useState({
    name: "",
    description: "",
    category: "",
    address: "",
    phone: "",
    email: "",
    website: "",
  })

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleCategoryChange = (value: string) => {
    setFormData((prev) => ({ ...prev, category: value }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setError(null)

    try {
      // Check if user is authenticated
      const {
        data: { session },
      } = await supabase.auth.getSession()

      if (!session) {
        router.push("/auth/signin?redirect=/businesses/add")
        return
      }

      // Get user details
      const { data: userData, error: userError } = await supabase
        .from("users")
        .select("*")
        .eq("id", session.user.id)
        .single()

      if (userError) throw userError

      // Insert business data
      const { error: businessError } = await supabase.from("businesses").insert([
        {
          user_id: session.user.id,
          name: formData.name,
          description: formData.description,
          category_id: formData.category,
          address: formData.address,
          phone: formData.phone,
          email: formData.email,
          website: formData.website,
          is_claimed: true,
        },
      ])

      if (businessError) throw businessError

      setSuccess(true)

      // Reset form
      setFormData({
        name: "",
        description: "",
        category: "",
        address: "",
        phone: "",
        email: "",
        website: "",
      })

      // Redirect after a delay
      setTimeout(() => {
        router.push("/businesses")
      }, 2000)
    } catch (error: any) {
      setError(error.message || "An error occurred while adding your business")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="container py-10">
      <div className="max-w-3xl mx-auto">
        <div className="mb-8 text-center">
          <h1 className="text-3xl font-bold">Add Your Business</h1>
          <p className="text-muted-foreground mt-2">List your business on dekat.me to reach more customers</p>
        </div>

        <Card>
          <form onSubmit={handleSubmit}>
            <CardHeader>
              <CardTitle>Business Information</CardTitle>
              <CardDescription>Provide details about your business to help customers find you</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              {error && <div className="bg-destructive/15 text-destructive p-3 rounded-md">{error}</div>}

              {success && (
                <div className="bg-green-500/15 text-green-500 p-3 rounded-md">
                  Your business has been successfully added! Redirecting...
                </div>
              )}

              <div className="space-y-2">
                <Label htmlFor="name">Business Name *</Label>
                <Input id="name" name="name" value={formData.name} onChange={handleChange} required />
              </div>

              <div className="space-y-2">
                <Label htmlFor="description">Description *</Label>
                <Textarea
                  id="description"
                  name="description"
                  value={formData.description}
                  onChange={handleChange}
                  rows={4}
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="category">Category *</Label>
                <Select value={formData.category} onValueChange={handleCategoryChange} required>
                  <SelectTrigger>
                    <SelectValue placeholder="Select a category" />
                  </SelectTrigger>
                  <SelectContent>
                    {categories.map((category) => (
                      <SelectItem key={category.id} value={category.id}>
                        {category.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="address">Address *</Label>
                <Textarea
                  id="address"
                  name="address"
                  value={formData.address}
                  onChange={handleChange}
                  rows={2}
                  required
                />
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="phone">Phone Number *</Label>
                  <Input id="phone" name="phone" value={formData.phone} onChange={handleChange} required />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="email">Email Address</Label>
                  <Input id="email" name="email" type="email" value={formData.email} onChange={handleChange} />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="website">Website</Label>
                <Input
                  id="website"
                  name="website"
                  type="url"
                  value={formData.website}
                  onChange={handleChange}
                  placeholder="https://"
                />
              </div>
            </CardContent>
            <CardFooter className="flex flex-col space-y-2">
              <Button type="submit" className="w-full" disabled={loading}>
                {loading ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Adding Business...
                  </>
                ) : (
                  "Add Business"
                )}
              </Button>
              <p className="text-xs text-muted-foreground text-center">* Required fields</p>
            </CardFooter>
          </form>
        </Card>
      </div>
    </div>
  )
}

