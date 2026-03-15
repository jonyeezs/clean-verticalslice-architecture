package demo.cleanslice.usecases.create_recipe;

import an.awesome.pipelinr.Command;
import java.util.Optional;
import java.util.concurrent.CompletableFuture;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Component;

@Component
public class CreateRecipeHandler
    implements Command.Handler<APIRequest, CompletableFuture<Response>>
{

    private DataAccess dataAccess;

    public CreateRecipeHandler(
        @Qualifier("CreateRecipe") DataAccess dataAccess
    ) {
        this.dataAccess = dataAccess;
    }

    @Override
    public CompletableFuture<Response> handle(APIRequest request) {
        try {
            var domain = this.dataAccess.Retrieve();

            var recipe = new Recipe(request.getTitle(), new Ingredient[0]);

            domain.addRecipe(recipe);

            var updatedDomain = this.dataAccess.Add(domain);

            return CompletableFuture.completedFuture(
                new Response(Optional.ofNullable(updatedDomain.get(0)))
            );
        } catch (Exception e) {
            return CompletableFuture.completedFuture(
                new Response(Optional.empty())
            );
        }
    }
}
