import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { Post } from 'src/app/_models/post';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { PostsService } from 'src/app/_services/posts.service';

@Component({
  selector: 'app-post-detail',
  templateUrl: './post-detail.component.html',
  styleUrls: ['./post-detail.component.css']
})
export class PostDetailComponent implements OnInit {
  post: Post | null = null;
  postUserId: number = 0;
  isEditable: boolean = false;
  user: User | null = null;

  constructor(private postService: PostsService,private accountService: AccountService, private route: ActivatedRoute) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadPost();
  }

  onRouteToLinkPost(){
    window.location.href = this.post?.linkForPost as string;
  }
  loadPost() {
    const postid = this.route.snapshot.paramMap.get('id');
    var postId: number = Number(postid);
    this.postService.getPostById(postId).subscribe({
      next: post => {
        if (post) {
          this.post = post;
          if (this.post.userName == this.user?.username){
            this.isEditable = true;
          }
        }
      }
    });
  }
}
