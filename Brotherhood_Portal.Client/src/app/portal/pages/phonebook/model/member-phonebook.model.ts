export interface MemberPhonebook {
  id: string;
  displayName: string;
  firstName?: string;
  lastName?: string;
  occupation?: string;
  business?: string;
  contactNumber: string;

  // Future Props
  // - Brotherhood Title / Responsibility / CommunityRole 
  // - Social Links
  // - Profile Image
}