export interface Profile {
  displayName: string;
  username: string;
  bio: string;
  image: string;
  photos: Photo[];
  following: boolean;
  followersCount: number;
  followingCount: number;
}

export interface Photo {
  id: string;
  url: string;
  isMain: boolean;
}
