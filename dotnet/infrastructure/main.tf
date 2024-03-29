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
  region  = "ap-southeast-2"
}

provider "aws" {
  region = local.region
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

output "image_repository_url" {
  value = aws_ecr_repository.dot-net-architecture.repository_url
}

output "iam_execution_task_arn" {
  value = aws_iam_role.task_execution.arn
}

output "aws_ecs_service" {
  value = aws_ecs_service.ecs_service.id
}

output "aws_ecs_init_task_definition_arn" {
  value = aws_ecs_task_definition.initiator.arn
}
