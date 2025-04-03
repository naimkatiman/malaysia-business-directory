export type Json = string | number | boolean | null | { [key: string]: Json | undefined } | Json[]

export interface Database {
  public: {
    Tables: {
      businesses: {
        Row: {
          id: string
          user_id: string
          name: string
          description: string
          category_id: string
          website: string | null
          phone: string | null
          email: string | null
          address: string
          location: any // PostGIS geography type
          is_premium: boolean
          is_verified: boolean
          is_claimed: boolean
          rating: number
          review_count: number
          operating_hours: Json | null
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          user_id: string
          name: string
          description: string
          category_id: string
          website?: string | null
          phone?: string | null
          email?: string | null
          address: string
          location: any
          is_premium?: boolean
          is_verified?: boolean
          is_claimed?: boolean
          rating?: number
          review_count?: number
          operating_hours?: Json | null
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          name?: string
          description?: string
          category_id?: string
          website?: string | null
          phone?: string | null
          email?: string | null
          address?: string
          location?: any
          is_premium?: boolean
          is_verified?: boolean
          is_claimed?: boolean
          rating?: number
          review_count?: number
          operating_hours?: Json | null
          created_at?: string
          updated_at?: string
        }
      }
      business_images: {
        Row: {
          id: string
          business_id: string
          url: string
          is_primary: boolean
          created_at: string
        }
        Insert: {
          id?: string
          business_id: string
          url: string
          is_primary?: boolean
          created_at?: string
        }
        Update: {
          id?: string
          business_id?: string
          url?: string
          is_primary?: boolean
          created_at?: string
        }
      }
      business_searches: {
        Row: {
          id: string
          query: string | null
          category_id: string | null
          location: any | null
          radius: number | null
          user_id: string | null
          searched_at: string
        }
        Insert: {
          id?: string
          query?: string | null
          category_id?: string | null
          location?: any | null
          radius?: number | null
          user_id?: string | null
          searched_at?: string
        }
        Update: {
          id?: string
          query?: string | null
          category_id?: string | null
          location?: any | null
          radius?: number | null
          user_id?: string | null
          searched_at?: string
        }
      }
      business_tags: {
        Row: {
          business_id: string
          tag_id: string
        }
        Insert: {
          business_id: string
          tag_id: string
        }
        Update: {
          business_id?: string
          tag_id?: string
        }
      }
      business_views: {
        Row: {
          id: string
          business_id: string
          user_id: string | null
          viewed_at: string
        }
        Insert: {
          id?: string
          business_id: string
          user_id?: string | null
          viewed_at?: string
        }
        Update: {
          id?: string
          business_id?: string
          user_id?: string | null
          viewed_at?: string
        }
      }
      categories: {
        Row: {
          id: string
          name: string
          description: string | null
          icon: string | null
          created_at: string
        }
        Insert: {
          id?: string
          name: string
          description?: string | null
          icon?: string | null
          created_at?: string
        }
        Update: {
          id?: string
          name?: string
          description?: string | null
          icon?: string | null
          created_at?: string
        }
      }
      reviews: {
        Row: {
          id: string
          business_id: string
          user_id: string
          rating: number
          content: string
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          business_id: string
          user_id: string
          rating: number
          content: string
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          business_id?: string
          user_id?: string
          rating?: number
          content?: string
          created_at?: string
          updated_at?: string
        }
      }
      spotlight_tokens: {
        Row: {
          id: string
          business_id: string
          redeemed: boolean
          redeemed_at: string | null
          expires_at: string
          created_at: string
        }
        Insert: {
          id?: string
          business_id: string
          redeemed?: boolean
          redeemed_at?: string | null
          expires_at: string
          created_at?: string
        }
        Update: {
          id?: string
          business_id?: string
          redeemed?: boolean
          redeemed_at?: string | null
          expires_at?: string
          created_at?: string
        }
      }
      subscriptions: {
        Row: {
          id: string
          business_id: string
          plan: string
          start_date: string
          end_date: string
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          business_id: string
          plan: string
          start_date: string
          end_date: string
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          business_id?: string
          plan?: string
          start_date?: string
          end_date?: string
          created_at?: string
          updated_at?: string
        }
      }
      tags: {
        Row: {
          id: string
          name: string
          created_at: string
        }
        Insert: {
          id?: string
          name: string
          created_at?: string
        }
        Update: {
          id?: string
          name?: string
          created_at?: string
        }
      }
      users: {
        Row: {
          id: string
          email: string
          name: string
          role: string
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          email: string
          name: string
          role?: string
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          email?: string
          name?: string
          role?: string
          created_at?: string
          updated_at?: string
        }
      }
    }
    Views: {
      [_ in never]: never
    }
    Functions: {
      businesses_within_distance: {
        Args: {
          lat: number
          lng: number
          distance_meters: number
        }
        Returns: {
          id: string
          user_id: string
          name: string
          description: string
          category_id: string
          website: string | null
          phone: string | null
          email: string | null
          address: string
          location: any
          is_premium: boolean
          is_verified: boolean
          is_claimed: boolean
          rating: number
          review_count: number
          operating_hours: Json | null
          created_at: string
          updated_at: string
        }[]
      }
    }
    Enums: {
      [_ in never]: never
    }
    CompositeTypes: {
      [_ in never]: never
    }
  }
}

