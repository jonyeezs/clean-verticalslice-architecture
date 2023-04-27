terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "4.52.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "3.4.3"
    }
  }
  required_version = ">= 1.1.0"

  cloud {
    organization = "whip-up"

    workspaces {
      name = "csharp-clean-architecture-github-action"
    }
  }
}

locals {
  service = "clean-slice"
}

provider "aws" {
  region = "ap-southeast-2"
  default_tags {
    tags = {
      Environment = "Production"
      Service     = local.service
    }
  }
}

output "url" {
  value = aws_lb.this.dns_name
}

output "image_repository" {
  value = aws_ecr_repository.dot-net-architecture.repository_url
}

output "iam_task_arn" {
  value = aws_iam_role.task.arn
}
