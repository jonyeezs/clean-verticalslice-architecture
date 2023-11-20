resource "aws_ecr_repository" "dot-net-architecture" {
  name                 = local.service
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }
}
