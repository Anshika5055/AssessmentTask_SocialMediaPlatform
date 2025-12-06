import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { PostComponent } from './post/post.component';
import { UserFeedComponent } from './user-feed/user-feed.component';
import { UserEngagementComponent } from './user-engagement/user-engagement.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: "Post", component: PostComponent},
    { path: "Feed", component: UserFeedComponent},
    { path: "Engagement", component: UserEngagementComponent}
];
