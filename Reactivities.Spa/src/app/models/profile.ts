export interface Profile {
    displayName: string,
    username: string,
    bio: string,
    image: string,
    photos: Photo[],
}

export interface Photo {
    id: string,
    url: string,
    isMain: boolean
}
