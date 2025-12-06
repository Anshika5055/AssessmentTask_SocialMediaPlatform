import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { User } from '../models/user';
import { Post } from '../models/post';
import { Like } from '../models/like';
import { Comment } from '../models/comment';
import { UserService } from '../services/user.service';
import { PostService } from '../services/post.service';
import { LikeService } from '../services/like.service';
import { CommentService } from '../services/comment.service';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';

interface UserEngagement {
  user: User;
  posts: number;
  likes: number;
  comments: number;
  score: number;
}

@Component({
  selector: 'app-user-engagement',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatCardModule],
  templateUrl: './user-engagement.component.html',
  styleUrl: './user-engagement.component.css'
})
export class UserEngagementComponent implements OnInit {
  userEngagements: UserEngagement[] = [];
  displayedColumns: string[] = ['userName', 'posts', 'likes', 'comments', 'score'];

  constructor(
    private userService: UserService,
    private postService: PostService,
    private likeService: LikeService,
    private commentService: CommentService
  ) {}

  ngOnInit(): void {
    this.loadEngagementData();
  }

  loadEngagementData(): void {
    let users: User[] = [];
    let posts: Post[] = [];
    let likes: Like[] = [];
    let comments: Comment[] = [];

    this.userService.getUsers().subscribe(usersData => {
      users = usersData;
      this.postService.getPosts().subscribe(postsData => {
        posts = postsData;
        this.likeService.getLikes().subscribe(likesData => {
          likes = likesData;
          this.commentService.getComments().subscribe(commentsData => {
            comments = commentsData;
            this.calculateEngagementScores(users, posts, likes, comments);
          });
        });
      });
    });
  }

  calculateEngagementScores(
    users: User[],
    posts: Post[],
    likes: Like[],
    comments: Comment[]
  ): void {
    this.userEngagements = users.map(user => {
      const userPosts = posts.filter(p => p.userID === user.userID).length;
      const userLikes = likes.filter(l => l.userID === user.userID).length;
      const userComments = comments.filter(c => c.userID === user.userID).length;
      
      const score = (userPosts * 5) + (userLikes * 2) + (userComments * 3);

      return {
        user: user,
        posts: userPosts,
        likes: userLikes,
        comments: userComments,
        score: score
      };
    });

    this.userEngagements.sort((a, b) => b.score - a.score);
  }
}

