resource "aws_ecs_cluster" "this" {
  name = "${local.service}-cluster"
}

resource "aws_ecs_task_definition" "this" {
  family             = "${local.service}-task"
  task_role_arn      = aws_iam_role.task.arn
  execution_role_arn = aws_iam_role.task.arn

  container_definitions = jsonencode(
    [{
      "name" : "my-api"
      "image" : "${data.aws_caller_identity.current.account_id}.dkr.ecr.${data.aws_region.current.name}.amazonaws.com/${local.service}",
      "portMappings" : [
        { containerPort = 80 }
      ],
    }]
  )

  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
}

resource "aws_ecs_service" "this" {
  name        = local.service
  cluster     = aws_ecs_cluster.this.id
  launch_type = "FARGATE"

  load_balancer {
    target_group_arn = aws_lb_target_group.this.arn
    container_port   = 80
    container_name   = local.service
  }

  task_definition = aws_ecs_task_definition.this.arn

  desired_count = 2

  network_configuration {
    subnets          = data.aws_subnet_ids.this.ids
    security_groups  = [aws_security_group.this.id]
    assign_public_ip = true
  }

  depends_on = [aws_lb.this]
}
