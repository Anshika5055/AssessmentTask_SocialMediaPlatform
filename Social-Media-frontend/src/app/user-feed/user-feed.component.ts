import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Post } from '../models/post';
import { Like } from '../models/like';
import { Comment } from '../models/comment';
import { User } from '../models/user';
import { PostService } from '../services/post.service';
import { LikeService } from '../services/like.service';
import { CommentService } from '../services/comment.service';
import { UserService } from '../services/user.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

interface FeedItem {
  type: 'post' | 'like' | 'comment';
  user: User;
  post: Post;
  like?: Like;
  comment?: Comment;
  timestamp: Date;
}

@Component({
  selector: 'app-user-feed',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule
  ],
  templateUrl: './user-feed.component.html',
  styleUrl: './user-feed.component.css'
})
export class UserFeedComponent implements OnInit {
  feedItems: FeedItem[] = [];
  posts: Post[] = [];
  likes: Like[] = [];
  comments: Comment[] = [];
  users: User[] = [];
  selectedUser: User | null = null;
  newComment: { [postId: number]: string } = {};
  moderationError: { [postId: number]: string } = {};

  private bannedWords = [
    'monolith', 'spaghettiCode', 'goto', 'hack', 'architrixs',
    'quickAndDirty', 'cowboy', 'yo', 'globalVariable', 'recursiveHell',
    'backdoor', 'hotfix', 'leakyAbstraction', 'mockup', 'singleton',
    'silverBullet', 'technicalDebt'
  ];

  constructor(
    private postService: PostService,
    private likeService: LikeService,
    private commentService: CommentService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.userService.selectedUser$.subscribe(user => {
      this.selectedUser = user;
    });
    this.loadData();
  }

  loadData(): void {
    this.postService.getPosts().subscribe(posts => {
      this.posts = posts;
      this.buildFeed();
    });

    this.likeService.getLikes().subscribe(likes => {
      this.likes = likes;
      this.buildFeed();
    });

    this.commentService.getComments().subscribe(comments => {
      this.comments = comments;
      this.buildFeed();
    });

    this.userService.getUsers().subscribe(users => {
      this.users = users;
      this.buildFeed();
    });
  }

  buildFeed(): void {
    if (this.posts.length === 0 || this.users.length === 0) return;

    this.feedItems = [];

    this.posts.forEach(post => {
      const user = this.users.find(u => u.userID === post.userID);
      if (user) {
        this.feedItems.push({
          type: 'post',
          user: user,
          post: post,
          timestamp: new Date()
        });
      }
    });

    this.likes.forEach(like => {
      const user = this.users.find(u => u.userID === like.userID);
      const post = this.posts.find(p => p.postID === like.postID);
      if (user && post) {
        this.feedItems.push({
          type: 'like',
          user: user,
          post: post,
          like: like,
          timestamp: new Date()
        });
      }
    });

    this.comments.forEach(comment => {
      const user = this.users.find(u => u.userID === comment.userID);
      const post = this.posts.find(p => p.postID === comment.postID);
      if (user && post) {
        this.feedItems.push({
          type: 'comment',
          user: user,
          post: post,
          comment: comment,
          timestamp: new Date()
        });
      }
    });

    this.feedItems.sort((a, b) => b.timestamp.getTime() - a.timestamp.getTime());
  }

  getUserName(userId: number): string {
    const user = this.users.find(u => u.userID === userId);
    return user ? user.userName : 'Unknown';
  }

  likePost(postId: number): void {
    if (!this.selectedUser) return;

    const existingLike = this.likes.find(
      l => l.postID === postId && l.userID === this.selectedUser!.userID
    );

    if (existingLike) {
      this.likeService.deleteLike(existingLike.likeID).subscribe(() => {
        this.loadData();
      });
    } else {
      const newLike: Like = {
        likeID: 0,
        userID: this.selectedUser.userID,
        postID: postId
      };
      this.likeService.createLike(newLike).subscribe(() => {
        this.loadData();
      });
    }
  }

  isPostLiked(postId: number): boolean {
    if (!this.selectedUser) return false;
    return this.likes.some(
      l => l.postID === postId && l.userID === this.selectedUser!.userID
    );
  }

  addComment(postId: number): void {
    if (!this.selectedUser) return;

    const commentText = this.newComment[postId]?.trim();
    if (!commentText) return;

    if (this.containsBannedWords(commentText)) {
      this.moderationError[postId] = 'Comment contains inappropriate content. Please remove banned words.';
      return;
    }

    this.moderationError[postId] = '';

    const newComment: Comment = {
      commentID: 0,
      userID: this.selectedUser.userID,
      postID: postId,
      content: commentText
    };

    this.commentService.createComment(newComment).subscribe(() => {
      this.newComment[postId] = '';
      this.loadData();
    });
  }

  containsBannedWords(text: string): boolean {
    const lowerText = text.toLowerCase();
    return this.bannedWords.some(word => lowerText.includes(word.toLowerCase()));
  }

  getCommentsForPost(postId: number): Comment[] {
    return this.comments.filter(c => c.postID === postId);
  }
}

